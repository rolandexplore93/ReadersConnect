using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReadersConnect.Application.Helpers.Configuration;
using System.Text;

namespace ReadersConnect.Web.Extensions.Middlewares
{
    public class SwaggerAuthorizedMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly string _swaggerKey;
        public SwaggerAuthorizedMiddleWare(RequestDelegate next, IOptions<JwtConfig> options)
        {
            _next = next;
            _swaggerKey = options.Value.SwaggerKey;
        }

        public async Task Invoke(HttpContext context)
        {
            var uri = context.Request.Path.ToString();  
            if (uri.StartsWith("/index.html"))
            {
                var param = context.Request.QueryString.Value;

                if (!param.Equals($"?key={_swaggerKey}"))
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";  
                    JObject jObject = new JObject()
                    {
                        {"access", "Incorrect password/Not Found" },

                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(jObject), Encoding.UTF8);
                    return;
                }
            }
            await _next.Invoke(context);
        }
    }
}
