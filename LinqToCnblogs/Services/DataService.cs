using System;
using System.Collections.Generic;
using System.Text;
using LinqToCnblogs.Models.Entities;
using System.Linq;
using System.Xml.Linq;

namespace LinqToCnblogs.Services
{
    public class DataService : IDataService
    {
        public IEnumerable<Post> GetData()
        {
            string url = @"http://wcf.open.cnblogs.com/blog/sitehome/recent/100000";
            var document = XDocument.Load(url);

            var elements = document.Root.Elements();
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
            return result;
        }
    }
}
