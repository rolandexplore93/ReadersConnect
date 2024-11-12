using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
builder.Services.SwaggerConfig();
//builder.Services.AddSingleton<ErrorHandlingMiddleWare>();
builder.Services.AddDbContextAndConfigurations(environment, configuration);
builder.Services.AddCors(c =>{ c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());});
builder.Services.AddDependencyInjection();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(configuration);
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddAuthorizationPolicies(); 

builder.Services.AddAutoMapper(typeof(ReadersConnect.Web.Automapper.MappingConfig), typeof(MappingConfig));

try
{
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
}
catch (Exception ex)
{
    Log.Fatal($"Error occured starting the application: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}