﻿using AutoMapper;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Domain.Models;
using ReadersConnect.Domain.Models.Identity;

namespace ReadersConnect.Application.Automapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, RegisterStaffRequestDTO>().ReverseMap();
            CreateMap<ApplicationUser, StaffRegistrationResponse>().ReverseMap();
            CreateMap<ApplicationUser, UserResponseDto>().ReverseMap();
            CreateMap<ApplicationUser, RegisterUserRequestDTO>().ReverseMap();
            CreateMap<Book, AddBookRequestDTO>().ReverseMap();
            CreateMap<Book, BookResponseDto>().ReverseMap();
            CreateMap<BookRequest, BookRequestDTO>().ReverseMap();
            CreateMap<BookRequest, BookRequestResponseDto>().ReverseMap();
        }
    }
}