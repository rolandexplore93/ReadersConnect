using AutoMapper;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Helpers.Common;
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
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "AutoMapper is not properly injected.");
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
                
                if (!users.Any())
                {
                    _logger.LogWarning("No User found");
                    return APIResponse<List<UserResponseDto>>.FailedResult($"No Users Registered yet. Contact Admin for further information.", HttpStatusCode.NotFound);
                }

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

        public async Task<NoDataAPIResponse> DeletePermissionAsync(int permissionId)
        {
            try
            {
                if (permissionId == 0)
                {
                    return NoDataAPIResponse.FailedResult("Invalid permission id.", HttpStatusCode.BadRequest);
                }

                var permission = await _unitOfWork.GetRepository<Permission>().GetSingleAsync(p => p.Id == permissionId);

                if (permission == null)
                {
                    return NoDataAPIResponse.FailedResult("Permission not found.", HttpStatusCode.NotFound);
                }

                _unitOfWork.GetRepository<Permission>().Remove(permission);
                _unitOfWork.SaveChanges();
                return NoDataAPIResponse.SuccessResult("Permission deleted successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return NoDataAPIResponse.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<NoDataAPIResponse> AssignRoleToUserAsync(AssignRoleRequestDTO request)
        {
            try
            {
                // Get user by id
                var user = await _unitOfWork.GetRepository<ApplicationUser>().GetSingleAsync(u => u.Id == request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User ID {UserId} not found", request.UserId);
                    return NoDataAPIResponse.FailedResult("User not found", HttpStatusCode.NotFound);
                }

                // Check if the role exists
                var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
                if (!roleExists)
                {
                    _logger.LogWarning("{RoleName} not found", request.RoleName);
                    return NoDataAPIResponse.FailedResult($"{request.RoleName} not found", HttpStatusCode.NotFound);
                }

                // Check if the user already has this role
                var userHasRole = await _userManager.IsInRoleAsync(user, request.RoleName);
                if (userHasRole)
                {
                    _logger.LogWarning("User already has access to {RoleName} role:", request.RoleName);
                    return NoDataAPIResponse.FailedResult($"User already has access to {request.RoleName} role:", HttpStatusCode.Conflict);
                }

                // Assign the role to the user
                var result = await _userManager.AddToRoleAsync(user, request.RoleName);
                if (!result.Succeeded)
                {
                    // Log each error from Identity
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error assigning role: {Error}", error.Description);
                    }
                    return NoDataAPIResponse.FailedResult("Failed to assign role", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("Assigned role '{RoleName}' to user '{UserId}'", request.RoleName, request.UserId);
                return NoDataAPIResponse.SuccessResult("Role assigned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while assigning role: {Message}", ex.Message);
                return NoDataAPIResponse.FailedResult("An error occurred while assigning role", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<NoDataAPIResponse> RegisterMembersAsync(RegisterUserRequestDTO requestDTO)
        {
            try
            {
                // Check if user exists using their username or email address
                var user = await _unitOfWork.GetRepository<ApplicationUser>().GetSingleAsync(u => u.UserName == requestDTO.UserName || u.Email == requestDTO.Email);
                if (user != null)
                {
                    _logger.LogWarning("User already exists {UserName}",  requestDTO.UserName + requestDTO.Email);
                    return NoDataAPIResponse.FailedResult("Your already have an account with us. Please contact admin to reset your password.", HttpStatusCode.Conflict);
                }

                // Map requestDTO to ApplicationUser
                ApplicationUser mapUser = _mapper.Map<ApplicationUser>(requestDTO);
                mapUser.IsActive = true;
                mapUser.EmailConfirmed = true;
                mapUser.PhoneNumberConfirmed = true;
                mapUser.CreatedAt = DateTime.UtcNow;
                mapUser.ModifiedAt = DateTime.UtcNow;

                // Create new user member
                var result = await _userManager.CreateAsync(mapUser, requestDTO.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating user: {Error}", error.Description);
                    }

                    // Handle result.Errors and send to user
                    var errorMessages = result.Errors.Select(error => error.Description).ToList();
                    string combinedErrors = string.Join("; ", errorMessages);
                    return NoDataAPIResponse.FailedResult($"Failed to create user. {combinedErrors}", HttpStatusCode.InternalServerError);
                }

                // Create "Member" role if it does not exist
                if (!await _roleManager.RoleExistsAsync("Member"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Member"));
                }

                // Assign "Member" role to new user
                var roleResult = await _userManager.AddToRoleAsync(mapUser, "Member");
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        _logger.LogError("Error assigning role to user: {Error}", error.Description);
                    }

                    // Handle result.Errors and send to user
                    var errorMessages = result.Errors.Select(error => error.Description).ToList();
                    string combinedErrors = string.Join("; ", errorMessages);
                    return NoDataAPIResponse.FailedResult($"Failed to assign role to user. {combinedErrors}", HttpStatusCode.InternalServerError);
                }

                _logger.LogInformation("User {UserName} registered successfully with role 'Member'", mapUser.UserName);
                return NoDataAPIResponse.SuccessResult("Your account has been created.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred setting new user account {Message}", ex.Message);
                return NoDataAPIResponse.FailedResult("An error setting up your account. Please contact Support.", HttpStatusCode.InternalServerError);
            }
        }
    }
}
