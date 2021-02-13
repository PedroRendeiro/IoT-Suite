using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoTSuite.Server.Filters
{
    internal class FixFromFormEncodingOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fromFormParameter = context.MethodInfo.GetParameters()
                .FirstOrDefault(p => p.IsDefined(typeof(FromFormAttribute), true));
            if (fromFormParameter == null) return;

            foreach (var key in operation.RequestBody.Content.Keys)
            {
                foreach (var key1 in operation.RequestBody.Content[key].Encoding.Keys)
                {
                    operation.RequestBody.Content[key].Encoding[key1].ContentType = "text/plain";
                }
            }
        }
    }
}
