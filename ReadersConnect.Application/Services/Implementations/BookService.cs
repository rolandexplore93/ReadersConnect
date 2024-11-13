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
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BookService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "AutoMapper is not properly injected.");
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<NoDataAPIResponse> AddBookAsync(AddBookRequestDTO requestDTO)
        {
            try
            {
                // Check if book exists using their ISBN and Title
                var book = await _unitOfWork.GetRepository<Book>().GetSingleAsync(u => u.ISBN == requestDTO.ISBN || u.Title == requestDTO.Title);
                if (book != null)
                {
                    _logger.LogInformation($"{requestDTO.Title} has been added before");
                    return NoDataAPIResponse.FailedResult($"{requestDTO.Title} has been added to the list of books in the system before. Please add a new book.", HttpStatusCode.Conflict);
                }

                // Map requestDTO to Book Model
                Book mapBook = _mapper.Map<Book>(requestDTO);

                // Add book to db
                //Book newPermission = new Permission { PermissionName = requestDTO.PermissionName };
                var result = await _unitOfWork.GetRepository<Book>().AddAndSaveChangesAsync(mapBook);
                if (result != null) return NoDataAPIResponse.SuccessResult($"{mapBook.Title} with {mapBook.ISBN} added to book list");

                _logger.LogInformation($"Error occured adding {mapBook.Title} with {mapBook.ISBN} to book list");
                return NoDataAPIResponse.FailedResult($"Error occured adding {mapBook.Title} with {mapBook.ISBN} to book list", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred adding book to database {Message}", ex.Message);
                return NoDataAPIResponse.FailedResult("Error Occured adding books. Please contact Support.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<List<BookResponseDto>>> GetBooksAsync()
        {
            try
            {
                var books = await _unitOfWork.GetRepository<Book>().GetAllAsync();

                if (!books.Any())
                {
                    _logger.LogWarning("No Book found");
                    return APIResponse<List<BookResponseDto>>.FailedResult($"No Books Added Yet. Contact Support to publish books.", HttpStatusCode.NotFound);
                }

                // Map db result to BookResponseDto
                List<BookResponseDto> mappedBooks = _mapper.Map<List<BookResponseDto>>(books);
                return APIResponse<List<BookResponseDto>>.SuccessResult(mappedBooks, "Books retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return APIResponse<List<BookResponseDto>>.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}
