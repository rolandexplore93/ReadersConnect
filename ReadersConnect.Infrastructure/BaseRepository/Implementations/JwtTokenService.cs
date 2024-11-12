using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.Helpers.Configuration;
using ReadersConnect.Application.Services.Implementations;
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
        private string? _Audience;

        public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _secretKey = _configuration.GetValue<string>("JWTsettings:Secret");
            _Issuer = _configuration.GetValue<string>("JWTsettings:ValidIssuer");
            _Audience = _configuration.GetValue<string>("JWTsettings:ValidAudience");
        }
        public async Task<string> GenerateTokenAsync(ApplicationUser user, string jwtTokenId)
        {
            // If user is found, generate login token with JwtSecurityTokenHandler
            var roles = await _userManager.GetRolesAsync(user);

            // JWTHandler requires the secret key which must be in byte. So, convert the secretKey from string to byte
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = _Issuer,
                Audience = _Audience,
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey); ;

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _Issuer,
                    ValidateAudience = true,
                    ValidAudience = _Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
