using ReadersConnect.Application.BaseInterfaces.IUnitOfWork;
using ReadersConnect.Application.Dtos;
using ReadersConnect.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        public Task<APIResponse<string>> CreatePermissionAsync(CreatePermissionDto permissionDto)
        {
            throw new NotImplementedException();
        }

        public Task<APIResponse<string>> CreateRoleAsync(CreateRoleDto roleDto)
        {
            throw new NotImplementedException();
        }

        
    }
}
