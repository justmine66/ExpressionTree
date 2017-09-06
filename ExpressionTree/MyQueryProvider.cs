using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionTree
{
    public class MyQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MyQueryable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return new List<object>();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            List<LambdaExpression> lambdas = null;
            AnalysisExpression.VisitExpression2(expression, ref lambdas);
            IEnumerable<Student> enumrable = null;
            foreach (var lambda in lambdas)
            {
                Func<Student, bool> func = (lambda as Expression<Func<Student, bool>>).Compile();
                if (enumrable == null)
                {
                    enumrable = Program.StudentArrary;
                }
                else
                {
                    enumrable = enumrable.Where(func);
                }
            }
            dynamic obj = enumrable.ToList();
            return obj;
        }
    }
}
