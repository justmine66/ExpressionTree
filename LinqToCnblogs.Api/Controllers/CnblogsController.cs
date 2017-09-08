using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinqToCnblogs.Api.Models.Entities;
using LinqToCnblogs.Api.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToCnblogs.Api.Controllers
{
    [Route("api/[controller]")]
    public class CnblogsController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<Post> Get(SearchCriteria criteria = null)
        {
            var result = PostManager.Posts;
            if (criteria != null)
            {
                if (!string.IsNullOrEmpty(criteria.Title))
                    result = result.Where(
                        p => p.Title.IndexOf(criteria.Title, StringComparison.OrdinalIgnoreCase) > 0);

                if (!string.IsNullOrEmpty(criteria.Author))
                    result = result.Where(
                        p => p.Author.IndexOf(criteria.Author, StringComparison.OrdinalIgnoreCase) > 0);
                //发布时间
                if (criteria.Start.HasValue)
                    result = result.Where(p => p.Published >= criteria.Start.Value);
                if (criteria.End.HasValue)
                    result = result.Where(p => p.Published <= criteria.End.Value);
                //评论数
                if (criteria.MinComments > 0)
                    result = result.Where(p => p.Comments >= criteria.MinComments);
                if (criteria.MaxComments > 0)
                    result = result.Where(p => p.Comments <= criteria.MaxComments);
                //访问量
                if (criteria.MinViews > 0)
                    result = result.Where(p => p.Diggs >= criteria.MinViews);
                if (criteria.MaxViews > 0)
                    result = result.Where(p => p.Diggs <= criteria.MaxViews);
                //推荐数
                if (criteria.MinDiggs > 0)
                    result = result.Where(p => p.Diggs >= criteria.MinDiggs);
                if (criteria.MaxDiggs > 0)
                    result = result.Where(p => p.Diggs <= criteria.MaxDiggs);
            }
            return result;
        }
    }
}
