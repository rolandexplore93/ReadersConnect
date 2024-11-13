using Microsoft.AspNetCore.Identity;
using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Infrastructure.Persistence;
using ReadersConnect.Infrastructure.UnitOfWork;
using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;
using ReadersConnect.Infrastructure.DbInitializer;
using ReadersConnect.Application.Services.Interfaces;
using ReadersConnect.Application.Services.Implementations;
using ReadersConnect.Infrastructure.BaseRepository.Implementations;

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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookService, BookService>();
            //services.AddScoped<IAdminService, AdminService>();


            // Add Repository Injections Here
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork<CoreApplicationContext>>();

            // Add Fluent Validator Injections Here
            //services.AddTransient<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
        }
    }
}
