using IoTSuite.Shared;
using IoTSuite.Shared.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTSuite.Server.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route, IQueryCollection queries);
    }
}
