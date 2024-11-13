using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<APIResponse<StaffRegistrationResponse>> RegisterStaffAsync(RegisterStaffRequestDTO requestDTO);
        Task<APIResponse<List<UserResponseDto>>> GetUsersAsync();
        Task<NoDataAPIResponse> AddRoleAsync(CreateRoleRequestDto requestDTO);
        Task<NoDataAPIResponse> AddPermissionAsync(CreatePermissionRequestDto requestDTO);
        Task<NoDataAPIResponse> DeletePermissionAsync(int permissionId);
        Task<NoDataAPIResponse> AssignRoleToUserAsync(AssignRoleRequestDTO requestDTO);
        Task<NoDataAPIResponse> RegisterMembersAsync(RegisterUserRequestDTO requestDTO);

    }
}
