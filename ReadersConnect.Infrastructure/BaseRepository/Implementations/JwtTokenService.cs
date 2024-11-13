using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.Helpers.Configuration;
using ReadersConnect.Application.Services.Interfaces;
using ReadersConnect.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Infrastructure.BaseRepository.Implementations
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private string? _secretKey;
        private string? _Issuer;
        //private string? _Audience;

        public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            //_secretKey = _configuration.GetValue<string>("Jwt:SigningKey");
            _secretKey = _configuration.GetValue<string>("JWTsettings:Secret");
            
            //_Issuer = _configuration.GetValue<string>("Jwt:Issuer");
            //_Audience = _configuration.GetValue<string>("Jwt:ValidAudience");
        }
        public async Task<string> GenerateTokenAsync(ApplicationUser user, string jwtTokenId)
        {
            // If user is found, generate login token with JwtSecurityTokenHandler
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any())
            {
                var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
            new Claim(ClaimTypes.Name, user.UserName.ToString()),
        };

                // Add all roles as claims
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                // Key and credentials for signing
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Create the token
                var token = new JwtSecurityToken(
                    issuer: "https://readersconnect-api.com",  // Ensure this matches the configuration
                    audience: "https://test-readersconnect-api.com",  // Ensure this matches the configuration
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            return string.Empty;

        }

        //public bool ValidateToken(string token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_secretKey); ;

        //    try
        //    {
        //        tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,
        //            ValidIssuer = _Issuer,
        //            ValidateAudience = true,
        //            ValidAudience = _Audience,
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);

        //        return validatedToken != null;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
