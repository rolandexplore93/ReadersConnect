using ReadersConnect.Web.Extensions.Middlewares;

namespace ReadersConnect.Web.Extensions
{
    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthorizedMiddleWare>();
        }
    }
}
