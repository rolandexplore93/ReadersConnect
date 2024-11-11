using Microsoft.AspNetCore.Identity;
using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Infrastructure.Persistence;
 using ReadersConnect.Infrastructure.UnitOfWork;
using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;

namespace ReadersConnect.Web.Extensions
{
    public static class DIServiceExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            // Add Identity Injections Here
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CoreApplicationContext>()
                .AddDefaultTokenProviders();

            // Add Service Injections Here
            //services.AddScoped<IAuthenticationService, AuthenticationService>();
            //services.AddScoped<IAdminService, AdminService>();


            // Add Repository Injections Here
            //services.AddSingleton<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork<CoreApplicationContext>>();

            // Add Fluent Validator Injections Here
            //services.AddTransient<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        }
    }
}
