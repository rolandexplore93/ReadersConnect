using Microsoft.AspNetCore.Mvc;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Services.Implementations;
using ReadersConnect.Application.Services.Interfaces;
using ReadersConnect.Web.Extensions.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace ReadersConnect.Web.Controllers
{
    [Route("api/v1/Books")]
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to get the list of all books")]
        [HttpGet("GetBooks")]
        [ProducesResponseType(typeof(APIResponse<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersAsync()
        {
            // Call GetUsersAsync method from _userService
            var result = await _bookService.GetBooksAsync();

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            if (result.HttpStatusCode != HttpStatusCode.OK)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows user to add a book to books list")]
        //[Authorize]
        [HttpPost("AddBook")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddBookAsync(AddBookRequestDTO requestDto)
        {
            var result = await _bookService.AddBookAsync(requestDto);

            if (result.HttpStatusCode == HttpStatusCode.Conflict)
            {
                return Conflict(result);
            }

            if (result.HttpStatusCode != HttpStatusCode.OK || result.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to borrow a book")]
        [HttpPost("RequestBook")]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestBookAsync([FromBody] BookRequestDTO requestDto)
        {
            var result = await _bookService.RequestBookAsync(requestDto);

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            if (result.HttpStatusCode != HttpStatusCode.OK || result.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to get the list of all books requested")]
        [HttpGet("GetBookRequests")]
        [ProducesResponseType(typeof(APIResponse<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllBookRequestsAsync()
        {
            // Call GetAllBookRequestsAsync method from _bookService
            var result = await _bookService.GetAllBookRequestsAsync();

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            if (result.HttpStatusCode != HttpStatusCode.OK || result.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to get a single book by parameter")]
        [HttpGet("GetBook/{bookId:int}")]
        [ProducesResponseType(typeof(APIResponse<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllBookRequestsAsync(int bookId)
        {
            // Call GetAllBookRequestsAsync method from _bookService
            var result = await _bookService.GetSingleBookRequestAsync(bookId);

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            if (result.HttpStatusCode != HttpStatusCode.OK || result.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to approve or reject borrowing request")]
        [HttpPost("ApproveOrRejectBookRequest")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveOrRejectBookRequestAsync([FromBody] ApproveOrRejectBookRequestDTO requestDto)
        {
            var result = await _bookService.ApproveOrRejectBookRequestAsync(requestDto);

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            if (result.HttpStatusCode != HttpStatusCode.OK || result.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
