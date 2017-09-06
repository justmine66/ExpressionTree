using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //part1();
            //part2();

            Console.Read();
        }

        public static void part1()
        {
            //var aa = new MyQueryable<Student>();
            //var bb = aa.Where(s => s.Name == "李军");
            //var cc = bb.Where(s => s.Age > 24);
            //var dd = cc.AsEnumerable();
            //var ee = dd.ToList();

            LabelTarget labelTarget = Expression.Label();
            ParameterExpression loopIndex = Expression.Parameter(typeof(int), "index");
            Expression.Assign(loopIndex, Expression.Constant(1));

            BlockExpression block = Expression.Block(
                new[] { loopIndex },
                Expression.Assign(loopIndex, Expression.Constant(1)),
                Expression.Loop(
                    Expression.IfThenElse(
                    Expression.LessThanOrEqual(loopIndex, Expression.Constant(10)),
                    Expression.Block(
                        Expression.Call(null,
                    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
                        Expression.Constant("hello")),
                        Expression.PostIncrementAssign(loopIndex)
                        ),
                    Expression.Break(labelTarget)
                    ), labelTarget));

            Expression<Action> lambda = Expression.Lambda<Action>(block);
            lambda.Compile()();

            Expression<Func<int, int>> func = x => x + 1;

            TypeBinaryExpression type = Expression.TypeIs(Expression.Constant("pro"), typeof(int));

            ParameterExpression arr = Expression.Parameter(typeof(int[]), "arr");
            ParameterExpression index = Expression.Parameter(typeof(int), "index");
            ParameterExpression value = Expression.Parameter(typeof(int), "value");

            Expression arrAccess = Expression.ArrayAccess(arr, index);

            Expression<Func<int[], int, int, int>> lambdaExpr =
                Expression.Lambda<Func<int[], int, int, int>>
                (
                    Expression.Assign(arrAccess, Expression.Add(arrAccess, value)),
                    arr, index, value
                );

            Console.WriteLine(arrAccess.ToString());
            Console.WriteLine(lambdaExpr.ToString());

            Console.WriteLine(lambdaExpr.Compile()(new int[] { 1, 2, 3 }, 0, 5));

            Expression<Func<int, int, bool>> largeSumTest = (num1, num2) => (num1 + num2) > 1000;
            InvocationExpression invocation = Expression.Invoke(
                largeSumTest,
                Expression.Constant(100),
                Expression.Constant(100));

            //ParameterExpression student = Expression.Parameter(typeof(Student));
            //var expr = Expression.Property(student, "Name");
            //var where = Expression.Equal(expr, Expression.Constant("小明"));
            //var orderExpr = Expression.Lambda<Func<Student, bool>>(where, student);

            //var orderedStudentArrary = StudentArrary.Where(orderExpr.Compile());

            var modules = new List<Module>{
                new Module{Name = "01"},
                new Module{Name = "02"},
                new Module{Name = "03"},
                new Module{Name = "04", IsDeleted = true},
                new Module{Name = "05"},
            };
            ParameterExpression Module = Expression.Parameter(typeof(Module));
            var proExpr = Expression.Property(Module, "Name");
            var condition = Expression.Equal(proExpr, Expression.Constant("02"));
            var proExprDel = Expression.Property(Module, "IsDeleted");
            var deleteCondition = Expression.Equal(proExprDel, Expression.Constant(false));

            var predicateExp = Expression.AndAlso(condition, deleteCondition);
            var predicate = Expression.Lambda<Func<Module, bool>>(predicateExp, Module).Compile();
            var filter = modules.Where(predicate);

        }

        public static void part2()
        {
            //直接返回常量值
            ConstantExpression conExpr = Expression.Constant("直接返回常量值");
            Expression<Func<string>> conReturnExpr = Expression.Lambda<Func<string>>(conExpr);
            Console.WriteLine(conReturnExpr.Compile()());
            //在方法体内创建变量，经过操作之后再返回
            ParameterExpression arg1 = Expression.Parameter(typeof(int));
            BlockExpression block1 = Expression.Block(
                new[] { arg1 },
                Expression.AddAssign(arg1, Expression.Constant(2, typeof(int)))
                );
            Expression<Func<int>> expr2 = Expression.Lambda<Func<int>>(block1);
            Console.WriteLine(expr2.Compile()());
            //利用GotoExpression返回值
            LabelTarget returnTarget = Expression.Label(typeof(int));
            LabelExpression returnLabel = Expression.Label(returnTarget, Expression.Constant(10, typeof(int)));
            ParameterExpression inPar = Expression.Parameter(typeof(int));
            BlockExpression block3 = Expression.Block(
                Expression.AddAssign(inPar, Expression.Constant(10, typeof(int))),
                Expression.Return(returnTarget, inPar), returnLabel
                );
            Expression<Func<int, int>> expr3 = Expression.Lambda<Func<int, int>>(block3, inPar);
            Console.WriteLine(expr3.Compile().Invoke(20));
            //简单的switch case 语句
            ParameterExpression genderParam = Expression.Parameter(typeof(int));
            SwitchExpression swichExpr = Expression.Switch(genderParam,
                Expression.Constant("不详"),
                Expression.SwitchCase(Expression.Constant("男"), Expression.Constant(1)),
                Expression.SwitchCase(Expression.Constant("女"), Expression.Constant(0)));
            Expression<Func<int, string>> expr4 = Expression.Lambda<Func<int, string>>(swichExpr, genderParam);
            Console.WriteLine(expr4.Compile().Invoke(1)); //男
            Console.WriteLine(expr4.Compile().Invoke(0)); //女
            Console.WriteLine(expr4.Compile().Invoke(11)); //不详
            //表达式访问
            var studentSql1 = StudentArrary.AsQueryable().Where(s => s.Age > 20);
            Console.WriteLine(studentSql1);
            var studentSql2 = StudentArrary.AsQueryable().Where(s => s.Name == "小明");
            Console.WriteLine(studentSql2);
        }

        public static List<Student> StudentArrary = new List<Student>()
        {
                new Student(){Name="小张", Age=26, Sex="男", Address="秀山"},
                new Student(){Name="小明", Age=23, Sex="男", Address="重庆"},
                new Student(){Name="王五", Age=25, Sex="女", Address="四川"}
        };
    }

    public class Module
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class Student
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }

        public string Sex { get; set; }
    }

}