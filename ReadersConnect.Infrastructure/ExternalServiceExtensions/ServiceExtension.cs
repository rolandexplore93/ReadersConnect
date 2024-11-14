//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.IdentityModel.Tokens;
//using ReadersConnect.Application.Helpers.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ReadersConnect.Infrastructure.ExternalServiceExtensions
//{
//    public static class ServiceExtension
//    {
//        //public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
//        //{
//        //    var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTsettings:Secret"));

//        //    // Configure authentication
//        //    services.AddAuthentication(options =>
//        //    {
//        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        //    })
//        //    .AddJwtBearer(options =>
//        //    {
//        //        options.RequireHttpsMetadata = false;
//        //        options.SaveToken = true;
//        //        options.TokenValidationParameters = new TokenValidationParameters
//        //        {
//        //            ValidateIssuerSigningKey = true,
//        //            IssuerSigningKey = new SymmetricSecurityKey(key),
//        //            ValidateIssuer = true,
//        //            ValidIssuer = configuration.GetValue<string>("JWTsettings:ValidIssuer"),
//        //            ValidAudience = configuration.GetValue<string>("JWTsettings:ValidAudience"),
//        //            ValidateAudience = true,
//        //            ClockSkew = TimeSpan.Zero
//        //        };
//        //    });

//        //    return services;
//        //}

//        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
//        {
//            string issuer = configuration.GetSection("Jwt:Issuer").Value;
//            string signingKey = configuration.GetSection("Jwt:SigningKey").Value;
//            string lifeTime = configuration.GetSection("Jwt:ExpiryInMinutes").Value;

//            var jwtSettings = new JwtSettings
//            {
//                SigningKey = signingKey,
//                ExpiryInMinutes = lifeTime,
//                Issuer = issuer
//            };

//            services.AddSingleton<JwtSettings>(jwtSettings);

//            return services;
//        }
//        public static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration)
//        {
//            string issuer = configuration.GetSection("Jwt:Issuer").Value;
//            string signingKey = configuration.GetSection("Jwt:SigningKey").Value;
//            string lifeTime = configuration.GetSection("Jwt:ExpiryInMinutes").Value;
//            string site = configuration.GetSection("Jwt:Site").Value;
//            var authenticationProviderKey = "Bearer";

//            var jwtSettings = new JwtSettings
//            {
//                SigningKey = signingKey,
//                ExpiryInMinutes = lifeTime,
//                Issuer = issuer,
//                Site = site
//            };

//            services.AddSingleton(jwtSettings);

//            var tokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuerSigningKey = true,
//                ValidateAudience = false,
//                ValidateIssuer = false,
//                RequireExpirationTime = true,
//                ValidateLifetime = true,
//                ValidIssuer = jwtSettings.Issuer,
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SigningKey)),
//                ClockSkew = TimeSpan.Zero
//            };
//            services.AddSingleton(tokenValidationParameters);

//            services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = authenticationProviderKey;
//                options.DefaultScheme = authenticationProviderKey;
//                options.DefaultChallengeScheme = authenticationProviderKey;
//                options.DefaultForbidScheme = authenticationProviderKey;
//            })
//                .AddJwtBearer(authenticationProviderKey, options =>
//                {
//                    options.RequireHttpsMetadata = false;
//                    options.SaveToken = true;
//                    options.TokenValidationParameters = tokenValidationParameters;

//                });

//            return services;
//        }
//    }
//}
