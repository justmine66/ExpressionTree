using LinqToCnblogs.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinqToCnblogs.Services
{
    public interface IDataService
    {
        IEnumerable<Post> GetData();
    }
}
