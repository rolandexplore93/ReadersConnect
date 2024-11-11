using AutoMapper;
using ReadersConnect.Application.Dtos;
using ReadersConnect.Domain.Models.Identity;

namespace ReadersConnect.Application.Automapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}