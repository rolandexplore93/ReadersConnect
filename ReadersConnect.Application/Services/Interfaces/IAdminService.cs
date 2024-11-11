using ReadersConnect.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Services.Interfaces
{
    public interface IAdminService
    {
        Task<APIResponse<string>> CreateRoleAsync(CreateRoleDto roleDto);
        Task<APIResponse<string>> CreatePermissionAsync(CreatePermissionDto permissionDto);
    }
}
