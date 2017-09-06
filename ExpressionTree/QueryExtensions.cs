using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionTree
{
    public static class QueryExtensions
    {
        public static string Where<TSource>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var expression = Expression.Call(
                null,
                typeof(QueryExtensions).GetMethod("Where").MakeGenericMethod(typeof(TSource)),
                new Expression[] { source.Expression, Expression.Quote(predicate) }
                );

            var translator = new QueryTranslator();
            return translator.Translate(expression);
        }
    }
}
