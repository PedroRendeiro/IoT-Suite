using System;
using IoTSuite.Server.Services;
using IoTSuite.Shared.Filters;
using System.Collections.Generic;
using IoTSuite.Shared.Wrappers;
using Microsoft.AspNetCore.Http;

namespace IoTSuite.Server.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter filter, int totalRecords, IUriService uriService, HttpRequest httpRequest)
        {
            var route = httpRequest.Path.Value;

            var response = new PagedResponse<List<T>>(pagedData, filter.PageNumber, filter.PageSize);
            
            var totalPages = ((double)totalRecords / (double)filter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            
            response.NextPage =
                filter.PageNumber >= 1 && filter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(filter.PageNumber + 1, filter.PageSize), route, httpRequest.Query)
                : null;
            
            response.PreviousPage =
                filter.PageNumber - 1 >= 1 && filter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(filter.PageNumber - 1, filter.PageSize), route, httpRequest.Query)
                : null;
            
            response.FirstPage = uriService.GetPageUri(new PaginationFilter(1, filter.PageSize), route, httpRequest.Query);
            response.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, filter.PageSize), route, httpRequest.Query);
            response.TotalPages = roundedTotalPages;
            response.TotalRecords = totalRecords;
            
            return response;
        }
    }
}
