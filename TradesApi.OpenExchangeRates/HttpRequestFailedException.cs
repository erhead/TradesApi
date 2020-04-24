using System;
using System.Net;

namespace TradesApi.OpenExchangeRates
{
    public class HttpRequestFailedException : Exception
    {
        public HttpStatusCode? StatusCode { get; private set; }

        public HttpRequestFailedException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpRequestFailedException(string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = null;
        }
    }
}
