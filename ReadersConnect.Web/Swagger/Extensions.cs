using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReadersConnect.Web.Swagger
{
    public static class SwaggerExtensions
    {

        public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            public void Configure(SwaggerGenOptions options)
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                                  "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
                                  "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "Bearer"
                },
                new List<string>()
            }
        });
            }
        }

        //public static void SwaggerConfig(this IServiceCollection services)
        //{
        //    services.AddSwaggerGen(c =>
        //    {
        //        c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReadersConnect", Version = "v1" });
        //        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //        {
        //            Name = "Authorization",
        //            Type = SecuritySchemeType.ApiKey,
        //            Scheme = "Bearer",
        //            BearerFormat = "JWT",
        //            In = ParameterLocation.Header,
        //            Description = "JWT Authorization header using the Bearer scheme."
        //        });
        //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //        {
        //            {
        //                new OpenApiSecurityScheme
        //                {
        //                    Reference = new OpenApiReference
        //                    {
        //                        Type =ReferenceType.SecurityScheme,
        //                        Id = "Bearer"
        //                    }
        //                },
        //                Array.Empty<string>()
        //            }
        //        });
        //    });
        //}


    }

    //public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    //{
    //    //readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;
    //    //public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _apiVersionDescriptionProvider = provider;

    //    public void Configure(SwaggerGenOptions options)
    //    {
    //        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //        {
    //            Description =
    //        "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
    //        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
    //        "Example: \"Bearer 1s233egfyw345bs\"",
    //            Name = "Authorization",
    //            In = ParameterLocation.Header,
    //            Scheme = "Bearer"
    //        });
    //        //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    //        //{
    //        //    Name = "Authorization",
    //        //    Type = SecuritySchemeType.ApiKey,
    //        //    Scheme = "Bearer",
    //        //    BearerFormat = "JWT",
    //        //    In = ParameterLocation.Header,
    //        //    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    //        //});
    //        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    //        {
    //            {
    //                new OpenApiSecurityScheme
    //                {
    //                    Reference = new OpenApiReference
    //                    {
    //                        Type = ReferenceType.SecurityScheme,
    //                        Id = "Bearer"
    //                    },
    //                    Scheme = "oauth2",
    //                    Name = "Bearer",
    //                    In = ParameterLocation.Header
    //                },
    //                new List<string>()
    //            }
    //        });


            

            //foreach (var desc in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            //{
            //    options.SwaggerDoc(desc.GroupName, new OpenApiInfo
            //    {
            //        Version = desc.ApiVersion.ToString(),
            //        Title = $"Villa Homes {desc.ApiVersion}",
            //        Description = "API to manage Villas",
            //        TermsOfService = new Uri("https://www.linkedin.com/in/orobola-roland-ogundipe/"),
            //        Contact = new OpenApiContact
            //        {
            //            Name = "Roland",
            //            Url = new Uri("https://www.linkedin.com/in/orobola-roland-ogundipe/")
            //        },
            //        License = new OpenApiLicense
            //        {
            //            Name = "RollyJS",
            //            Url = new Uri("https://www.linkedin.com/in/orobola-roland-ogundipe/")
            //        }
            //    });
            //};
        //}
    //}
}
