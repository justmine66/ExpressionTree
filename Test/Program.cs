using LinqToCnblogs.Api.CnblogsLinqProvider;
using LinqToCnblogs.Api.Models.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new CnblogsQueryProvider();
            var queryable = new Query<Post>(provider);

            var query = from p in queryable
                        where p.Title.Contains("001") &&
                        p.Diggs >= 10 &&
                        p.Comments > 10 &&
                        p.Views > 10 &&
                        p.Comments < 20
                        select p;

            //var list = query.ToList();

            TypeSystem.GetNonNullableType(typeof(Post));

            Console.Read();
        }
    }
}