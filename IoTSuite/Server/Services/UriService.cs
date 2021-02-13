using IoTSuite.Shared;
using IoTSuite.Shared.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IoTSuite.Server.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        public Uri GetPageUri(PaginationFilter filter, string route, IQueryCollection queries)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));

            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "PageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "PageSize", filter.PageSize.ToString());

            foreach (var query in queries)
            {
                if (query.Key.ToLower() == "pagenumber" | query.Key.ToLower() == "pagesize")
                {
                    continue;
                }
                else
                {
                    modifiedUri = QueryHelpers.AddQueryString(modifiedUri, query.Key, query.Value);
                }
            }
            
            return new Uri(modifiedUri);
        }
    }
}
