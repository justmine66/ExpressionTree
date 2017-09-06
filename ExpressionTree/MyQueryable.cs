using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionTree
{
    public class MyQueryable<T> : IQueryable<T>
    {
        public MyQueryable()
        {
            _expression = Expression.Constant(this);
            _provider = new MyQueryProvider();
        }

        public MyQueryable(Expression expression)
        {
            _expression = expression;
            _provider = new MyQueryProvider();
        }

        public Type ElementType => typeof(T);

        private Expression _expression { get; set; }
        public Expression Expression => _expression;

        public IQueryProvider _provider;
        public IQueryProvider Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            var result = _provider.Execute<List<T>>(_expression);
            if (result == null) yield break;
            foreach (var item in result)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_provider.Execute(_expression) as IEnumerable).GetEnumerator();
        }
    }
}
