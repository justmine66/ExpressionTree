using LinqToCnblogs.Api.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace LinqToCnblogs.Api.CnblogsLinqProvider
{
    public class PostHelper
    {
        static internal string BuildUrl(SearchCriteria criteria, string url = null)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var sbUrl = new StringBuilder(url ?? "");
            var sbParameter = new StringBuilder();
            if (!String.IsNullOrEmpty(criteria.Title))
                AppendParameter(sbParameter, "Title", criteria.Title);

            if (!String.IsNullOrEmpty(criteria.Author))
                AppendParameter(sbParameter, "Author", criteria.Author);

            if (criteria.Start.HasValue)
                AppendParameter(sbParameter, "Start", criteria.Start.Value.ToString());

            if (criteria.End.HasValue)
                AppendParameter(sbParameter, "End", criteria.End.Value.ToString());

            if (criteria.MinDiggs > 0)
                AppendParameter(sbParameter, "MinDiggs", criteria.MinDiggs.ToString());

            if (criteria.MinViews > 0)
                AppendParameter(sbParameter, "MinViews", criteria.MinViews.ToString());

            if (criteria.MinComments > 0)
                AppendParameter(sbParameter, "MinComments",
                    criteria.MinComments.ToString());

            if (criteria.MaxDiggs > 0)
                AppendParameter(sbParameter, "MaxDiggs", criteria.MaxDiggs.ToString());

            if (criteria.MaxViews > 0)
                AppendParameter(sbParameter, "MaxViews", criteria.MaxViews.ToString());

            if (criteria.MaxComments > 0)
                AppendParameter(sbParameter, "MaxComments",
                    criteria.MaxComments.ToString());

            if (sbParameter.Length > 0)
                sbUrl.AppendFormat("?{0}", sbParameter.ToString());

            return sbUrl.ToString();
        }

        static internal void AppendParameter(StringBuilder sb, string name, string value)
        {
            if (sb.Length > 0)
                sb.Append("&");

            sb.AppendFormat("{0}={1}", name, value);
        }

        static internal async Task<IEnumerable<Post>> PerformWebQueryAsync(string url)
        {
            var request = WebRequest.Create(url);

            var response = await request.GetResponseAsync();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var body = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Post>>(body);
            }
        }
    }
}
