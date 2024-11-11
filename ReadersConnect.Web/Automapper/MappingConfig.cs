using AutoMapper;
using ReadersConnect.Application.Dtos;
using ReadersConnect.Domain.Models;
using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Web.Dtos;

namespace ReadersConnect.Web.Automapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<CreateRoleRequestDto, CreateRoleDto>().ReverseMap();
            CreateMap<CreatePermissionRequestDto, CreatePermissionDto>().ReverseMap();
            CreateMap<Permission, PermissionResponseDto>().ReverseMap();
            CreateMap<Permission, UpdatePermissionRequestDto>().ReverseMap();
        }
    }
}