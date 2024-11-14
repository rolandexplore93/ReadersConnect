using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Responses
{
    public class NoDataAPIResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseDescription { get; set; }
        public string? Data { get; set; } = null;
        public bool IsSuccess { get; set; }

        // Define Success Response
        public static NoDataAPIResponse SuccessResult(string message)
        {
            return new NoDataAPIResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseDescription = message,
                Data = default,
                IsSuccess = true
            };
        }

        // Define Failure Response
        public static NoDataAPIResponse FailedResult(string message, HttpStatusCode statusCode)
        {
            return new NoDataAPIResponse
            {
                HttpStatusCode = statusCode,
                ResponseDescription = message,
                Data = default,
                IsSuccess = false
            };
        }
    }
}
