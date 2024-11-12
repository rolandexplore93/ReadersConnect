using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Responses
{
    public class APIResponse<T> where T : class
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseDescription { get; set; }
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }

        // Define Success Response
        public static APIResponse<T> SuccessResult(T data, string message)
        {
            return new APIResponse<T>
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseDescription = message,
                Data = data,
                IsSuccess = true
            };
        }

        // Define Failure Response
        public static APIResponse<T> FailedResult(string message, HttpStatusCode statusCode)
        {
            return new APIResponse<T>
            {
                HttpStatusCode = statusCode,
                ResponseDescription = message,
                Data = default,
                IsSuccess = false
            };
        }
    }
}
