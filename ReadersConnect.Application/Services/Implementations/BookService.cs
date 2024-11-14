using AutoMapper;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<NoDataAPIResponse> ApproveOrRejectBookRequestAsync(ApproveOrRejectBookRequestDTO requestDto)
        {
            try
            {
                // Validate Status input by admin
                if (string.IsNullOrWhiteSpace(requestDto.Status) || !(requestDto.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || requestDto.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogInformation("Invalid Status");
                    return NoDataAPIResponse.FailedResult($"Invalid Status. Please enter 'Approved' or 'Rejected'", HttpStatusCode.BadRequest);
                }

                // Check requestId is not 0
                if (requestDto.RequestId <= 0)
                {
                    _logger.LogInformation("Invalid Request ID");
                    return NoDataAPIResponse.FailedResult($"Invalid Request ID. Please enter a valid Request ID", HttpStatusCode.BadRequest);
                }

                // Check user is a valid user
                var validUser = await _unitOfWork.GetRepository<ApplicationUser>().GetSingleAsync(p => p.Id == requestDto.ApprovedBy);

                if (validUser == null)
                {
                    _logger.LogInformation("Invalid user");
                    return NoDataAPIResponse.FailedResult($"You do not have account with us. Enter correct userid", HttpStatusCode.NotFound);
                }

                // Get the book requested by ID
                var bookRequested = await _unitOfWork.GetRepository<BookRequest>().GetSingleAsync(p => p.RequestId == requestDto.RequestId);
                if (bookRequested == null)
                {
                    _logger.LogInformation("Invalid Book Request ID");
                    return NoDataAPIResponse.FailedResult($"Invalid Book Request ID.", HttpStatusCode.NotFound);
                }

                // If book has been approved before, do not update it
                if (bookRequested.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) || bookRequested.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"Request previously {bookRequested.Status.ToLower()}.");
                    return NoDataAPIResponse.FailedResult($"You have previously {bookRequested.Status.ToLower()} this request.", HttpStatusCode.BadRequest);
                }

                // Update the status of the request
                bookRequested.Status = requestDto.Status;
                _unitOfWork.SaveChanges();

                // For approved request, store the record to BorrowingRecord table
                if (requestDto.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                {
                    var borrowingRecord = new BorrowingRecord
                    {
                        RequestId = bookRequested.RequestId,
                        ApprovedBy = requestDto.ApprovedBy,
                        ReturnedDate = (DateTime)bookRequested.ReturnedDate
                    };

                    await _unitOfWork.GetRepository<BorrowingRecord>().AddAndSaveChangesAsync(borrowingRecord);
                }

                return NoDataAPIResponse.SuccessResult($"Book request has been {requestDto.Status.ToLower()}.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return NoDataAPIResponse.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<List<BookRequestResponseDto>>> GetAllBookRequestsAsync()
        {
            try
            {
                var booksRequests = await _unitOfWork.GetRepository<BookRequest>().GetAllAsync(p => p.Status != "", includeProperties: "Book");
                if (!booksRequests.Any())
                {
                    _logger.LogInformation("No Books Requested yet");
                    return APIResponse<List<BookRequestResponseDto>>.FailedResult($"No Books has been requested for borrowing yet.", HttpStatusCode.NotFound);
                }

                // Map db result to BookRequestResponseDto
                List<BookRequestResponseDto> bookRequestDtos = _mapper.Map<List<BookRequestResponseDto>>(booksRequests);

                return APIResponse<List<BookRequestResponseDto>>.SuccessResult(bookRequestDtos, "Books retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return APIResponse<List<BookRequestResponseDto>>.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<List<BookResponseDto>>> GetBooksAsync()
        {
            try
            {
                var books = await _unitOfWork.GetRepository<Book>().GetAllAsync();

                if (!books.Any())
                {
                    _logger.LogInformation("No Books found");
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

        public async Task<APIResponse<BookRequestResponseDto>> GetSingleBookRequestAsync(int bookId)
        {
            try
            {
                var bookRequest = await _unitOfWork.GetRepository<BookRequest>().GetSingleAsync(p => p.BookId == bookId, includeProperties: "Book");
                if (bookRequest == null)
                {
                    _logger.LogInformation("Book ID is not valid");
                    return APIResponse<BookRequestResponseDto>.FailedResult($"BookID is not valid", HttpStatusCode.NotFound);
                }

                // Map db result to BookRequestResponseDto
                BookRequestResponseDto bookRequestDto = _mapper.Map<BookRequestResponseDto>(bookRequest);

                return APIResponse<BookRequestResponseDto>.SuccessResult(bookRequestDto, "Book retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return APIResponse<BookRequestResponseDto>.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<BookRequestResponseDto>> RequestBookAsync(BookRequestDTO requestDto)
        {
            try
            {
                // Check return date is not in past
                if (requestDto.ReturnedDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Request failed: Returned data cannot be in the past");
                    return APIResponse<BookRequestResponseDto>.FailedResult($"Returned date cannot be in the past. Enter future date of when you will return this book. Date formate is yyyy-MM-ddTHH:mm:ssZ", HttpStatusCode.BadRequest);
                }

                // Check user is a valid user
                var validUser = await _unitOfWork.GetRepository<ApplicationUser>().GetSingleAsync(p => p.Id == requestDto.ApplicationUserId);

                if (validUser == null)
                {
                    _logger.LogInformation("Invalid user");
                    return APIResponse<BookRequestResponseDto>.FailedResult($"You do not have account with us. Enter correct userid", HttpStatusCode.NotFound);
                }

                // Search if the book is in bookstore
                var book = await _unitOfWork.GetRepository<Book>().GetSingleAsync(p => p.BookId == requestDto.BookId);

                if (book == null)
                {
                    _logger.LogInformation("No Book found");
                    return APIResponse<BookRequestResponseDto>.FailedResult($"Book not found.", HttpStatusCode.NotFound);
                }

                // Check if the book is available (i.e Copies must but greater or equal to 1)
                if (book.Copies <= 0)
                {
                    _logger.LogInformation("This book is currently not available for borrowing.");
                    return APIResponse<BookRequestResponseDto>.FailedResult($"{book.Title} by {book.Author} is currently not available for borrowing.", HttpStatusCode.NotFound);
                }

                // Map db request dto to BookResponse Model
                BookRequest mappedBooks = _mapper.Map<BookRequest>(requestDto);
                await _unitOfWork.GetRepository<BookRequest>().AddAndSaveChangesAsync(mappedBooks);

                // Map BookResponse model to BookRequestResponseDto
                BookRequestResponseDto bookResponseDto = _mapper.Map<BookRequestResponseDto>(mappedBooks);

                return APIResponse<BookRequestResponseDto>.SuccessResult(bookResponseDto, $"You have requested for {bookResponseDto.Book.Title}. We will contact you via phone call for collection once your requests is approved.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {Errors}", ex.Message);
                return APIResponse<BookRequestResponseDto>.FailedResult($"Error occured: {ex.Message}", HttpStatusCode.InternalServerError);
            }
        }
    }
}
