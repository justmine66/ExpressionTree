using LinqToCnblogs.Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LinqToCnblogs.Api.CnblogsLinqProvider
{
    /// <summary>
    /// 帖子表达式访问器
    /// </summary>
    public class PostExpressionVisitor
    {
        //搜索条件
        private SearchCriteria _criteria;

        /// <summary>
        /// 翻译表达式树成搜索条件---入口方法
        /// </summary>
        /// <param name="expression">表达式树</param>
        /// <returns>搜索条件</returns>
        public SearchCriteria ProcessExpression(Expression expression)
        {
            this._criteria = new SearchCriteria();
            this.VisitExpression(expression);
            return this._criteria;
        }

        //访问表达式树
        private void VisitExpression(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.AndAlso:// 访问 &&
                    this.VisitAndAlso(expr as BinaryExpression);
                    break;
                case ExpressionType.Call:// 访问调用方法，主要有于解析Contains方法，我们的Title会用到
                    this.VisitMethodCall(expr as MethodCallExpression);
                    break;
                case ExpressionType.Equal:// 访问 ==
                    this.VisitEqual(expr as BinaryExpression);
                    break;
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:// 访问大于和大于等于
                    this.VisitGreaterThanOrEqual(expr as BinaryExpression);
                    break;
                case ExpressionType.Lambda:// 访问Lambda表达式
                    this.VisitExpression((expr as LambdaExpression).Body);
                    break;
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:// 访问 小于和小于等于
                    this.VisitLessThanOrEqual(expr as BinaryExpression);
                    break;
                default:
                    throw new NotSupportedException($"Expression type not supported{expr.NodeType}");
            }
        }

        //访问 &&
        private void VisitAndAlso(BinaryExpression andAlsoExpr)
        {
            this.VisitExpression(andAlsoExpr.Left);
            this.VisitExpression(andAlsoExpr.Right);
        }

        //访问 ==
        private void VisitEqual(BinaryExpression equalExpr)
        {
            // 我们这里面只处理在Author上的等于操作
            // Views, Comments, 和 Diggs 我们都是用的大于等于，或者小于等于
            if (equalExpr.Left.NodeType == ExpressionType.MemberAccess &&
                (equalExpr.Left as MemberExpression).Member.Name == "Author")
            {
                if (equalExpr.Right.NodeType == ExpressionType.Constant)
                {
                    _criteria.Author = (equalExpr.Right as ConstantExpression).Value + string.Empty;
                }
                else if (equalExpr.Right.NodeType == ExpressionType.MemberAccess)
                {
                    _criteria.Author = this.GetMemberValue(equalExpr.Right as MemberExpression) + string.Empty;
                }
                else
                {
                    throw new NotSupportedException($"Expression type not supported for author: {equalExpr.Right.NodeType.ToString()}");
                }
            }
        }

        //访问 >=
        private void VisitGreaterThanOrEqual(BinaryExpression expr)
        {
            // 处理 Diggs >= n  推荐人数
            this.SetMemberIntValueFromExpr("MinDiggs", "Diggs", expr);
            // 处理 Views >= n   访问人数
            this.SetMemberIntValueFromExpr("MinViews", "Views", expr);
            // 处理 Views >= n   评论数
            this.SetMemberIntValueFromExpr("MinComments", "Comments", expr);
            // 处理 发布时间 >=
            this.SetMemberDTValueFromExpr("Start", "Published", expr);
        }

        //访问 <=
        private void VisitLessThanOrEqual(BinaryExpression expr)
        {
            // 处理 Diggs <= n  推荐人数
            this.SetMemberIntValueFromExpr("MaxDiggs","Diggs", expr);
            // 处理 Views <= n   访问人数
            this.SetMemberIntValueFromExpr("MaxViews", "Views", expr);
            // 处理 Views <= n   评论数
            this.SetMemberIntValueFromExpr("MaxComments", "Comments", expr);
            // 处理 发布时间 <=
            this.SetMemberDTValueFromExpr("End", "Published", expr);
        }

        // 访问 方法调用
        private void VisitMethodCall(MethodCallExpression expr)
        {
            if (expr.Method.DeclaringType == typeof(Queryable) &&
                expr.Method.Name == "Where")
            {
                this.VisitExpression((expr.Arguments[1] as UnaryExpression).Operand);
            }
            else if (expr.Method.DeclaringType == typeof(string) &&
                expr.Method.Name == "Contains")
            {
                // Handle Title.Contains("")
                if (expr.Object.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression memberExpr = expr.Object as MemberExpression;
                    if (memberExpr.Expression.Type == typeof(Post))
                    {
                        if (memberExpr.Member.Name == "Title")
                        {
                            Expression argument;
                            argument = expr.Arguments[0];
                            if (argument.NodeType == ExpressionType.Constant)
                                _criteria.Title = (argument as ConstantExpression).Value + string.Empty;
                            else if (argument.NodeType == ExpressionType.MemberAccess)
                            {
                                _criteria.Title = this.GetMemberValue(argument as MemberExpression) + string.Empty;
                            }
                            else
                            {
                                throw new NotSupportedException($"Expression type not supported:{argument.NodeType.ToString()}");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Method not supported: {expr.Method.Name}");
            }
        }

        //根据成员名和表达式,设置成员数值
        private void SetMemberIntValueFromExpr(string propName, string memberName, BinaryExpression expr)
        {
            if (expr.Left.NodeType == ExpressionType.MemberAccess &&
                   (expr.Left as MemberExpression).Member.Name == memberName)
            {
                if (expr.Right.NodeType == ExpressionType.Constant)
                {
                    if (int.TryParse((expr.Right as ConstantExpression).Value + string.Empty, out int result))
                        this.SetPropertyValue(propName, result);
                }
                else if (expr.Right.NodeType == ExpressionType.MemberAccess)
                {
                    if (int.TryParse(this.GetMemberValue(expr.Right as MemberExpression) + string.Empty, out int result))
                        this.SetPropertyValue(propName, result);
                }
                else
                {
                    throw new NotSupportedException($"Expression type not supported for {memberName}: {expr.Right.NodeType.ToString()}");
                }
            }
        }

        //根据成员名和表达式,设置成员时间
        private void SetMemberDTValueFromExpr(string propName, string memberName, BinaryExpression expr)
        {
            if (expr.Left.NodeType == ExpressionType.MemberAccess &&
                   (expr.Left as MemberExpression).Member.Name == memberName)
            {
                if (expr.Right.NodeType == ExpressionType.Constant)
                {
                    if (DateTime.TryParse((expr.Right as ConstantExpression).Value + string.Empty, out DateTime result))
                        this.SetPropertyValue(propName, result);
                }
                else if (expr.Right.NodeType == ExpressionType.MemberAccess)
                {
                    if (DateTime.TryParse(this.GetMemberValue(expr.Right as MemberExpression) + string.Empty, out DateTime result))
                        this.SetPropertyValue(propName, result);
                }
                else
                {
                    throw new NotSupportedException($"Expression type not supported for {memberName}: {expr.Right.NodeType.ToString()}");
                }
            }
        }

        //设置属性值
        private void SetPropertyValue(string name, object value)
        {
            foreach (var pro in _criteria.GetType().GetProperties())
                if (pro.Name == name)
                {
                    pro.SetValue(_criteria, value);
                    break;
                }
        }

        //获取成员表达式值
        private object GetMemberValue(MemberExpression memberExpr)
        {
            MemberInfo memberInfo;
            object obj;

            if (memberExpr == null)
                throw new ArgumentNullException(nameof(memberExpr));

            if (memberExpr.Expression is ConstantExpression)
                obj = (memberExpr.Expression as ConstantExpression).Value;
            else if (memberExpr.Expression is MemberExpression)
                obj = this.GetMemberValue(memberExpr.Expression as MemberExpression);
            else
                throw new NotSupportedException($"Expression type is not supported:{memberExpr.Expression.GetType().FullName}");

            memberInfo = memberExpr.Member;
            if (memberInfo is PropertyInfo)
            {
                PropertyInfo property = memberInfo as PropertyInfo;
                return property.GetValue(obj);
            }
            else if (memberInfo is FieldInfo)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                return field.GetValue(obj);
            }
            else
            {
                throw new NotSupportedException($"MemberInfo type not supported:{memberInfo.GetType().FullName}");
            }
        }
    }
}
