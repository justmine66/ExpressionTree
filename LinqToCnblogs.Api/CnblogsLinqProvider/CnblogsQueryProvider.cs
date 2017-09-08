using LinqToCnblogs.Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinqToCnblogs.Api.CnblogsLinqProvider
{
    public class CnblogsQueryProvider : QueryProvider
    {
        public override object Execute(Expression expression)
        {
            string url = this.GetQueryText(expression);
            IEnumerable<Post> post = PostHelper.PerformWebQueryAsync(url).Result;
            return post;
        }

        public override string GetQueryText(Expression expression)
        {
            SearchCriteria criteria;

            //翻译查询条件
            criteria = new PostExpressionVisitor().ProcessExpression(expression);

            //生成URL
            string url = PostHelper.BuildUrl(criteria, @"http://localhost/api/Cnblogs");

            return url;
        }
    }
}
