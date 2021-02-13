using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IoTSuite.Shared;
using Microsoft.AspNetCore.Diagnostics;
using IoTSuite.Shared.Wrappers;

namespace IoTSuite.Server.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class Errors : ControllerBase
    {
        [Route("{code}")]
        public IActionResult Error(int code)
        {
            Response.StatusCode = code;

            var response = new Response<Exception>((HttpStatusCode)code);

            if (code == 401)
            {
                var header = Response.Headers["WWW-Authenticate"].ToString().Replace("\"", "").Split(",");
                if (header.Length > 1)
                {
                    response.Errors = new Dictionary<string, string[]>
                    {
                        { "Invalid Token", new string[] { header.Last().Split("=").Last() } }
                    };
                }
            }

            return new ObjectResult(response);
        }
    }
}
