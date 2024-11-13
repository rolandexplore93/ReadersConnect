using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using ReadersConnect.Application.Automapper;
using ReadersConnect.Application.Helpers.Common;
using ReadersConnect.Infrastructure.DbInitializer;
using ReadersConnect.Infrastructure.ExternalServiceExtensions;
using ReadersConnect.Infrastructure.Persistence;
using ReadersConnect.Web.Extensions;
using ReadersConnect.Web.Extensions.Middlewares;
using ReadersConnect.Web.Swagger;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using static ReadersConnect.Web.Swagger.SwaggerExtensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;

configuration.SetBasePath(environment.ContentRootPath)
                      .AddJsonFile("appsettings.json")
                      .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
                      .AddJsonFile($"secrets/appsettings.{environment}.json", optional: true)
                      .AddEnvironmentVariables();

// Configuring Serilog for logging
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.

//builder.Services.AddSingleton<ErrorHandlingMiddleWare>();
builder.Services.AddDbContextAndConfigurations(environment, configuration);
builder.Services.AddCors(c =>{ c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());});
builder.Services.AddDependencyInjection();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SchemaFilter<EnumSchemaFilter>();
//});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGenNewtonsoftSupport();

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTsettings:Secret"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JWTsettings:ValidIssuer"),
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("JWTsettings:ValidAudience"),
        ValidateLifetime = true,
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }
    });
});


//builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
//builder.Services.AddSwaggerGen();


//builder.Services.AddAuth(configuration);


//builder.Services.AddJWT(configuration);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

//builder.Services.AddAuthorizationPolicies();

builder.Services.AddAutoMapper(typeof(ReadersConnect.Application.Automapper.MappingConfig), typeof(MappingConfig));

//try
//{
    Log.Information("Starting ReadersConnect application...");

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
    {
        //app.UseSwaggerAuthorized();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReadersConnect API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    await ApplyDatabaseInitializerAsync();

    app.UseCors("AllowOrigin");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware(typeof(ErrorHandlingMiddleWare));
    app.MapControllers();

    app.Run();

    async Task ApplyDatabaseInitializerAsync()
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.InitializeDatabase();
            await dbInitializer.SeedSuperAdminUserAsync();
        }
    }
//}
//catch (Exception ex)
//{
//    Log.Fatal($"Error occured starting the application: {ex.Message}");
//}
//finally
//{
//    Log.CloseAndFlush();
//}