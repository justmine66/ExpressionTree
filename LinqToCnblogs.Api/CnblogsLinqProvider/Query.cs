using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LinqToCnblogs.Api.CnblogsLinqProvider
{
    public class Query<T> :
        IQueryable<T>, IQueryable,
        IEnumerable<T>, IEnumerable,
        IOrderedQueryable<T>, IOrderedQueryable
    {
        QueryProvider _provider;
        Expression _expr;

        public Query(QueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            this._provider = provider;
            this._expr = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentNullException(nameof(expression));

            this._provider = provider;
            this._expr = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression => this._expr;

        public IQueryProvider Provider => this._provider;

        public IEnumerator<T> GetEnumerator()
        {
            return (this._provider.Execute(_expr) as IEnumerable<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this._provider.Execute(this._expr)).GetEnumerator();
        }
    }
}
