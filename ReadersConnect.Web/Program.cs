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
using ReadersConnect.Application.Helpers.Configuration;
using ReadersConnect.Infrastructure.DbInitializer;
using ReadersConnect.Infrastructure.Persistence;
using ReadersConnect.Web.Extensions;
using ReadersConnect.Web.Extensions.Middlewares;
using Serilog;
using System.Text;

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

builder.Services.AddDbContextAndConfigurations(environment, configuration);
builder.Services.AddCors(c =>{ c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());});
builder.Services.AddDependencyInjection();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

//builder.Services.AddAuthorizationPolicies();
builder.Services.AddAutoMapper(typeof(MappingConfig));

try
{
    Log.Information("Starting ReadersConnect application...");

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen(swagger =>
    {
        //This is to generate the Default UI of Swagger Documentation
        swagger.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ReadersConnect System",
            Description = "APIs to manage a book library for Educational Development Trust"
        });
    
        //Enable authorization using Swagger (JWT)
        swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        });
        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}

                }
            });
    });


    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    
    // Seed data in the database
    builder.Services.AddScoped<SeedDataIntoDb>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
    {
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

    app.UseMiddleware<JwtMiddleware>();
    //app.UseMiddleware(typeof(ErrorHandlingMiddleWare));

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    async Task ApplyDatabaseInitializerAsync()
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.InitializeDatabase();
            await dbInitializer.SeedSuperAdminUserAsync();
            var seedDataIntoDB = scope.ServiceProvider.GetRequiredService<SeedDataIntoDb>();
            await seedDataIntoDB.SeedAsync();
        }
    }
}
catch (Exception ex)
{
    Log.Fatal($"Error occured starting the application: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}