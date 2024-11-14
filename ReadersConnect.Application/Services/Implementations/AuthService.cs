using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Services.Interfaces;
using ReadersConnect.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ILogger<AuthService> logger, IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }


        public async Task<APIResponse<LoginTokenDTO>> LoginAsync(LoginRequestDTO requestDTO)
        {
            try
            {
                _logger.LogInformation("User attempt to login...");
                if (string.IsNullOrEmpty(requestDTO.Username) || string.IsNullOrEmpty(requestDTO.Password))
                {
                    _logger.LogError("Login failed. No credentials submitted.");
                    return APIResponse<LoginTokenDTO>.FailedResult("Invalid credentials", HttpStatusCode.BadRequest);
                }
                var user = await _unitOfWork.GetRepository<ApplicationUser>().GetSingleAsync(u => u.UserName == requestDTO.Username);
                bool isValidPassword = await _userManager.CheckPasswordAsync(user, requestDTO.Password);

                if (user == null || isValidPassword == false)
                {
                    _logger.LogError("Login failed. Incorrect credentials.");
                    LoginTokenDTO response = new LoginTokenDTO() { AccessToken = "empty" };
                    return APIResponse<LoginTokenDTO>.FailedResult("Incorrect login credentials", HttpStatusCode.BadRequest);
                };

                // Generete login access token from _jwtTokenService
                _logger.LogInformation("Generating access token...");
                //var tokenId = $"JTI{Guid.NewGuid()}";
                string token = await _jwtTokenService.GenerateTokenAsync(user);

                if (string.IsNullOrEmpty(token))
                {
                    return APIResponse<LoginTokenDTO>.FailedResult("Login failed... Contact Admin to assign role to your account.", HttpStatusCode.BadRequest);
                }

                _logger.LogInformation("Login successful.");
                LoginTokenDTO loginToken = new LoginTokenDTO() { AccessToken = token };
                return APIResponse<LoginTokenDTO>.SuccessResult(loginToken, "Request successful.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured logging in: {ex.Message}");
                return APIResponse<LoginTokenDTO>.FailedResult(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
