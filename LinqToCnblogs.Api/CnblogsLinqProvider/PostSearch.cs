using LinqToCnblogs.Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Linq.Expressions;

namespace LinqToCnblogs.Api.CnblogsLinqProvider
{
    public class PostSearch : IEnumerable<Post>
    {
        private SearchCriteria _criteria;

        public PostSearch Where(Expression<Func<Post, bool>> predicate)
        {
            _criteria = new PostExpressionVisitor()
                .ProcessExpression(predicate);
            return this;
        }

        public PostSearch Select<TResult>(Expression<Func<Post, TResult>> selector)
        {
            return this;
        }

        public IEnumerator<Post> GetEnumerator()
        {
            return (this as IEnumerable).GetEnumerator() as IEnumerator<Post>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            string url = PostHelper.BuildUrl(_criteria);
            IEnumerable<Post> posts = PostHelper.PerformWebQueryAsync(url).Result;

            return posts.GetEnumerator();
        }
    }
}
