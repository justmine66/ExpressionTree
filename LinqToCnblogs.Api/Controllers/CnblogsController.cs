using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinqToCnblogs.Api.Models.Entities;
using LinqToCnblogs.Api.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToCnblogs.Api.Controllers
{
    [Route("api/[controller]")]
    public class CnblogsController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<Post> Get(SearchCriteria criteria = null)
        {
            var result = PostManager.Posts;
            if (criteria != null)
            {
                if (!string.IsNullOrEmpty(criteria.Title))
                    result = result.Where(
                        p => p.Title.IndexOf(criteria.Title, StringComparison.OrdinalIgnoreCase) > 0);

                if (!string.IsNullOrEmpty(criteria.Author))
                    result = result.Where(
                        p => p.Author.IndexOf(criteria.Author, StringComparison.OrdinalIgnoreCase) > 0);
                //发布时间
                if (criteria.Start.HasValue)
                    result = result.Where(p => p.Published >= criteria.Start.Value);
                if (criteria.End.HasValue)
                    result = result.Where(p => p.Published <= criteria.End.Value);
                //评论数
                if (criteria.MinComments > 0)
                    result = result.Where(p => p.Comments >= criteria.MinComments);
                if (criteria.MaxComments > 0)
                    result = result.Where(p => p.Comments <= criteria.MaxComments);
                //访问量
                if (criteria.MinViews > 0)
                    result = result.Where(p => p.Diggs >= criteria.MinViews);
                if (criteria.MaxViews > 0)
                    result = result.Where(p => p.Diggs <= criteria.MaxViews);
                //推荐数
                if (criteria.MinDiggs > 0)
                    result = result.Where(p => p.Diggs >= criteria.MinDiggs);
                if (criteria.MaxDiggs > 0)
                    result = result.Where(p => p.Diggs <= criteria.MaxDiggs);
            }
            return result;
        }

        /// <summary>
        /// The property value of an object is assigned to another object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source">Original object</param>
        /// <param name="result">New object</param>
        /// <param name="exceptions">Some properties doesn't need to change</param>
        protected void Transform<TSource, TResult, TProperty>(TSource source, TResult result, params Expression<Func<TSource, TProperty>>[] exceptions)
        {
            var exceptionalProps = new List<PropertyInfo>();
            foreach (var expr in exceptions)
            {
                MemberExpression memberExpr = null;
                switch (expr.Body.NodeType)
                {
                    case ExpressionType.Convert:
                        memberExpr = ((expr.Body as UnaryExpression).Operand as MemberExpression);
                        break;
                    case ExpressionType.MemberAccess:
                        memberExpr = expr.Body as MemberExpression;
                        break;
                }

                if (memberExpr != null)
                {
                    exceptionalProps.Add(memberExpr.Member as PropertyInfo);
                }
            }

            List<PropertyInfo> resultProps = result.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            List<PropertyInfo> sourceProps = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            sourceProps = sourceProps.Except(exceptionalProps).ToList();
            sourceProps.ForEach(s =>
            {
                var resultProp = resultProps.SingleOrDefault(r => r.Name == s.Name);
                if (resultProp != null)
                {
                    resultProp.SetValue(result,s.GetValue(source));
                }
            });
        }
    }
}
