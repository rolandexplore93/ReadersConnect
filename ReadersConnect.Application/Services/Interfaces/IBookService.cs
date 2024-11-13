﻿using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<NoDataAPIResponse> AddBookAsync(AddBookRequestDTO requestDTO);
        Task<APIResponse<List<BookResponseDto>>> GetBooksAsync();

    }
}