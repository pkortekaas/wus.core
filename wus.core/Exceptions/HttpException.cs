using System;
using System.Net;

namespace CoreWUS
{
    public sealed class HttpException: Exception
    {
        public int Code { get; private set; }

        public HttpException() : base() {}
        public HttpException(string message) : base(message) {}
        public HttpException(string message, Exception innerException) : base(message, innerException) {}

        public HttpException(HttpStatusCode statusCode)
            : base(statusCode.ToString())
        {
            Code = (int)statusCode;
        }
    }
}