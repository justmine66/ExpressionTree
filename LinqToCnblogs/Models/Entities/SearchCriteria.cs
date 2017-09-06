using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToCnblogs.Models.Entities
{
    public class SearchCriteria
    {
        public string Title { get; set; }
        public string Author { get; set; }

        //发布时间
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        //推荐数
        public int MinDiggs { get; set; }
        public int MaxDiggs { get; set; }

        //访问数
        public int MinViews { get; set; }
        public int MaxViews { get; set; }

        //评论数
        public int MinComments { get; set; }
        public int MaxComments { get; set; }
    }
}
