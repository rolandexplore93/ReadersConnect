using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Services.Interfaces;
using ReadersConnect.Domain.Models;
using ReadersConnect.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<NoDataAPIResponse> AddPermissionAsync(CreatePermissionRequestDto requestDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestDTO.PermissionName))
                {
                    return NoDataAPIResponse.FailedResult("All fields are required", HttpStatusCode.BadRequest);
                }

                // Check if permission exists
                bool DoesPermissionExists = await IsPermissionNameExistsAsync(requestDTO.PermissionName);
                if (DoesPermissionExists) return NoDataAPIResponse.FailedResult("Permission already exists.", HttpStatusCode.Conflict);

                // Create and add permission to db
                Permission newPermission = new Permission { PermissionName = requestDTO.PermissionName };
                var result = await _unitOfWork.GetRepository<Permission>().AddAndSaveChangesAsync(newPermission);
                if (result != null) return NoDataAPIResponse.SuccessResult($"{requestDTO.PermissionName} permission added successfully");

                return NoDataAPIResponse.FailedResult("Error Created Permission", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return NoDataAPIResponse.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<NoDataAPIResponse> AddRoleAsync(CreateRoleRequestDto requestDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestDTO.RoleName))
                {
                    return NoDataAPIResponse.FailedResult("All fields are required", HttpStatusCode.BadRequest);
                }

                // Check if role exists
                if (await _roleManager.RoleExistsAsync(requestDTO.RoleName))
                {
                    return NoDataAPIResponse.FailedResult("Role already exists.", HttpStatusCode.Conflict);
                }
            
                // Create new role
                var result = await _roleManager.CreateAsync(new IdentityRole(requestDTO.RoleName));
                if (result.Succeeded)
                {
                    return NoDataAPIResponse.SuccessResult($"{requestDTO.RoleName} role created successfully");
                }

                var errorMessages = result.Errors.Select(error => error.Description).ToList();
                string errors = string.Join("; ", errorMessages);
                return NoDataAPIResponse.FailedResult($"Error creating a new role. {errors}", HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return NoDataAPIResponse.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<List<UserResponseDto>>> GetUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.GetRepository<ApplicationUser>().GetAllAsync();
                List<UserResponseDto> mappedUsers = _mapper.Map<List<UserResponseDto>>(users);
                return APIResponse<List<UserResponseDto>>.SuccessResult(mappedUsers, "Users retrieved successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return APIResponse<List<UserResponseDto>>.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<StaffRegistrationResponse>> RegisterStaffAsync(RegisterStaffRequestDTO requestDTO)
        {
            try
            {
                _logger.LogInformation("User attempting to create staff user account...");

                // Check input fields are filled
                if (string.IsNullOrEmpty(requestDTO.Username) || string.IsNullOrEmpty(requestDTO.Password) || string.IsNullOrEmpty(requestDTO.FirstName) || string.IsNullOrEmpty(requestDTO.LastName) || string.IsNullOrEmpty(requestDTO.Gender))
                {
                    _logger.LogError("Registration failed. All fields are not supplied");
                    return APIResponse<StaffRegistrationResponse>.FailedResult("All fields are required", HttpStatusCode.BadRequest);
                }

                // Check if the username already exists
                //var existingUser = await _userManager.FindByNameAsync(requestDTO.Username);
                var existingUser = await _unitOfWork.GetRepository<ApplicationUser>().GetSingleAsync(u => u.UserName == requestDTO.Username);
                if (existingUser != null)
                {
                    return APIResponse<StaffRegistrationResponse>.FailedResult("Username already exists!", HttpStatusCode.Conflict);
                }

                // Create new staff member
                // Convert requestDTO to ApplicationUser Model object
                ApplicationUser userObj = _mapper.Map<ApplicationUser>(requestDTO);
                userObj.CreatedAt = DateTime.Now;
                userObj.ModifiedAt = DateTime.Now;
                userObj.IsActive = true;

                var result = await _userManager.CreateAsync(userObj, requestDTO.Password);

                if (result.Succeeded)
                {
                    // Convert from model to DTO response
                    StaffRegistrationResponse newUserReponse = _mapper.Map<StaffRegistrationResponse>(userObj);
                    _logger.LogInformation($"Staff member {userObj.UserName} created by SuperAdmin");
                    return APIResponse<StaffRegistrationResponse>.SuccessResult(newUserReponse, "Staff account created successfully.");
                }

                // Handle result.Errors and send to user
                var errorMessages = result.Errors.Select(error => error.Description).ToList();
                string combinedErrors = string.Join("; ", errorMessages);

                // Log errors
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error creating staff member: {Error}", error.Description);
                }

                return APIResponse<StaffRegistrationResponse>.FailedResult($"Error creating a staff member account. {combinedErrors}", HttpStatusCode.BadRequest);

            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create staff member: {Errors}", ex.Message);
               return APIResponse<StaffRegistrationResponse>.FailedResult("Failed to create staff member", HttpStatusCode.InternalServerError);
            }
        }

        private async Task<bool> IsPermissionNameExistsAsync(string permissionName)
        {
            var existingPermission = await _unitOfWork.GetRepository<Permission>().GetSingleAsync(p => p.PermissionName == permissionName);
            return existingPermission != null;
        }
    }
}
