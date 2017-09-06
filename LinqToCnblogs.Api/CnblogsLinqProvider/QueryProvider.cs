using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinqToCnblogs.Api.CnblogsLinqProvider
{
    public abstract class QueryProvider : IQueryProvider
    {
        public QueryProvider() { }
        public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return Activator.CreateInstance(
                           typeof(Query<>).MakeGenericType(elementType),
                           new object[] { this, expression }
                       ) as IQueryable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return  (TResult)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }
    }
}
