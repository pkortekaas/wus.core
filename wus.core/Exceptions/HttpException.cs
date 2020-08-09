using System;
using System.Net;

namespace CoreWUS
{
    public class HttpException: Exception
    {
        public int Code { get; private set; }

        public HttpException(HttpStatusCode statusCode)
            : base(statusCode.ToString())
        {
            Code = (int)statusCode;
        }
    }
}