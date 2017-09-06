using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionTree
{
    public static class AnalysisExpression
    {
        public static void VisitExpression2(Expression expression, ref List<LambdaExpression> lambdaOut)
        {
            if (lambdaOut == null)
            {
                lambdaOut = new List<LambdaExpression>();
            }
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    MethodCallExpression method = expression as MethodCallExpression;
                    Console.WriteLine("方法名:" + method.Method.Name);
                    for (int i = 0; i < method.Arguments.Count; i++)
                        VisitExpression2(method.Arguments[i], ref lambdaOut);
                    break;
                case ExpressionType.Lambda:
                    LambdaExpression lambda = expression as LambdaExpression;
                    lambdaOut.Add(lambda);
                    VisitExpression2(lambda.Body, ref lambdaOut);
                    break;
                case ExpressionType.Equal:
                case ExpressionType.AndAlso:
                    BinaryExpression binary = expression as BinaryExpression;
                    VisitExpression2(binary.Left, ref lambdaOut);
                    VisitExpression2(binary.Right, ref lambdaOut);
                    break;
                case ExpressionType.Constant:
                    ConstantExpression constant = expression as ConstantExpression;
                    break;
                case ExpressionType.MemberAccess:
                    MemberExpression Member = expression as MemberExpression;
                    break;
                case ExpressionType.Quote:
                    UnaryExpression unary = expression as UnaryExpression;
                    VisitExpression2(unary.Operand, ref lambdaOut);
                    break;
                default:
                    Console.WriteLine("UnKnow");
                    break;
            }
        }

        public static void VisitExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    break;
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                    BinaryExpression binary = expression as BinaryExpression;
                    Console.WriteLine($"运算符{binary.NodeType.ToString()}");
                    VisitExpression(binary.Left);
                    VisitExpression(binary.Right);
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.Call:
                    MethodCallExpression method = expression as MethodCallExpression;
                    Console.WriteLine($"方法名：{method.Method.Name}");
                    foreach (var arg in method.Arguments)
                    {
                        VisitExpression(arg);
                    }
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    ConstantExpression constant = expression as ConstantExpression;
                    Console.WriteLine($"常量值：{constant.Value}");
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Divide:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.GreaterThan:
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.IsFalse:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.Lambda:
                    LambdaExpression lambda = expression as LambdaExpression;
                    VisitExpression(lambda.Body);
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.LessThan:
                    break;
                case ExpressionType.LessThanOrEqual:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.MemberAccess:
                    MemberExpression member = expression as MemberExpression;
                    Console.WriteLine($"字段名称：{member.Member.Name},类型：{member.Member.MemberType}");
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.Multiply:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.MultiplyChecked:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.NotEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.Or:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.OrElse:
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.Power:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Subtract:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.Unbox:
                    break;
                default:
                    break;
            }
        }
    }
}
