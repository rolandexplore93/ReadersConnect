using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.Helpers.Configuration;

//using ReadersConnect.Application.Helpers.Configuration;
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
        private readonly AppSettings _appSettings;
        private string? _secretKey;

        public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _secretKey = _configuration.GetValue<string>("JWTsettings:Secret");
        }
        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {

            //Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);


            //    // If user is found, generate login token with JwtSecurityTokenHandler
            //    var roles = await _userManager.GetRolesAsync(user);

            //    if (roles.Any())
            //    {
            //        var claims = new List<Claim>
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            //    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
            //    new Claim(ClaimTypes.Name, user.UserName.ToString()),
            //};

            //        // Add all roles as claims
            //        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            //        foreach (var role in roles)
            //        {
            //            claims.Add(new Claim(ClaimTypes.Role, role));
            //        }

            //        // Key and credentials for signing
            //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //        // Create the token
            //        var token = new JwtSecurityToken(
            //            issuer: "https://readersconnect-api.com",
            //            audience: "https://test-readersconnect-api.com",
            //            claims: claims,
            //            expires: DateTime.UtcNow.AddMinutes(60),
            //            signingCredentials: creds
            //        );

            //        return new JwtSecurityTokenHandler().WriteToken(token);
            //    }

            //    return string.Empty;

        }
    }
}
