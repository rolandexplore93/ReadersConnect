using System.Net;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using ReadersConnect.Application.Helpers.Common;

namespace ReadersConnect.Web.Extensions.Middlewares
{
    public class ErrorHandlingMiddleWare
    {
        public readonly RequestDelegate _next;

        public ErrorHandlingMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //Log Exception on ur Preferred environment using custom exception Helper
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            var result = JsonConvert.SerializeObject(new ErrorResponse
            {
                ErrorDescription = ex.Message,  
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            //Add Logs when Log is implemented
            return context.Response.WriteAsync(result);
        }
    }
}
