using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IoTSuite.Shared.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data)
        {
            Data = data;
        }

        public Response(T data, HttpStatusCode statusCode) : this(data)
        {
            StatusCode = (int)statusCode;
        }

        public Response(HttpStatusCode statusCode)
        {
            Succeeded = false;

            StatusCode = (int)statusCode;
        }

        public Response(HttpStatusCode statusCode, string error) : this(statusCode)
        {
            Errors = new Dictionary<string, string[]>();

            Errors.Add(error, null);
        }

        public Response(HttpStatusCode statusCode, Dictionary<string, string[]> errors) : this(statusCode)
        {
            Errors = errors;            
        }

        public bool Succeeded { get; set; } = true;
        public T Data { get; set; }
        public int StatusCode { get; set; } = 200;
        public string Message
        {
            get
            {
                return ((HttpStatusCode)StatusCode).ToString();
            }
        }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
