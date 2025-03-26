
using System.Net;

namespace foodOrderingApp.Services
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public bool Success { get; set; }

        public AppException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
            Success = false;
        }
    }
}