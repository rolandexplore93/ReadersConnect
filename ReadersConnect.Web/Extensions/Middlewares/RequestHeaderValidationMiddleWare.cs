using Microsoft.Extensions.Options;

namespace ReadersConnect.Web.Extensions.Middlewares
{
    /// <summary>
    /// Encapsulates gthe validation of request header in the MiddleWare
    /// </summary>
    public class RequestHeaderValidationMiddleWare
    {
        private readonly RequestDelegate _next;

        public RequestHeaderValidationMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        //public async Task Invoke(HttpContext context, IOptions<>)
    }
}
