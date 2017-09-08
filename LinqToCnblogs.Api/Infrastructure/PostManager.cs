using LinqToCnblogs.Api.Models.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinqToCnblogs.Api.Infrastructure
{
    public class PostManager
    {
        private static List<Post> _posts;
        private static DateTime _lastModified = DateTime.UtcNow;
        private static volatile object _obj = new object();
        private static readonly string _serviceUrl;

        static PostManager()
        {
            _serviceUrl = "http://wcf.open.cnblogs.com/blog/sitehome/recent/20";

            //初始加载
            loadPostsFromCnblogs();
        }

        public static IEnumerable<Post> Posts
        {
            get
            {
                // 一分钟之后再次去博客园取最新的数据
                if (DateTime.UtcNow.AddMinutes(1) > _lastModified)
                {
                    loadPostsFromCnblogs();
                }
                return _posts;
            }
        }

        private static void loadPostsFromCnblogs()
        {
            lock (_obj)
            {
                _posts = new List<Post>();

                var content = ResponseContentAsync(_serviceUrl).Result;

                var document = XElement.Parse(content);

                var elements = document.Elements();
                var result = from entry in elements
                             where entry.HasElements
                             select new Post
                             {
                                 Id = Convert.ToInt32(entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "id").Value),

                                 Title = entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "title").Value,

                                 Published = Convert.ToDateTime(entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "published").Value),

                                 Diggs = Convert.ToInt32(entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "diggs").Value),

                                 Views = Convert.ToInt32(entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "views").Value),

                                 Comments = Convert.ToInt32(entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "comments").Value),

                                 Summary = entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "summary").Value,

                                 Href = entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "link")
                            .Attribute("href").Value,

                                 Author = entry.Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "author")
                            .Elements()
                            .SingleOrDefault(x => x.Name.LocalName == "name").Value
                             };

                _posts.AddRange(result);
                _lastModified = DateTime.UtcNow;
            }
        }

        private static async Task<string> ResponseContentAsync(string uri)
        {
            using (var httClient = new HttpClient())
            {
                return await httClient.GetStringAsync(uri);
            }
        }
    }
}
