using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Services.Implementations;
using ReadersConnect.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace ReadersConnect.Web.Controllers
{
    [Route("api/v1/Users")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        //[SwaggerOperation(Summary = "Description: This endpoint allows super admin to create a staff account")]
        ////[Authorize(Roles = "SuperAdmin")]
        //[HttpPost("RegisterStaff")]
        //[ProducesResponseType(typeof(APIResponse<StaffRegistrationResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> RegisterStaffAsync([FromBody] RegisterStaffRequestDTO requestDTO)
        //{
        //    // Call LoginAsync method from AuthService
        //    var result = await _userService.RegisterStaffAsync(requestDTO);

        //    if (result.HttpStatusCode == HttpStatusCode.Conflict)
        //    {
        //        return Conflict(result);
        //    }

        //    if (result.HttpStatusCode != HttpStatusCode.OK || result.HttpStatusCode == HttpStatusCode.BadRequest)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}

        [SwaggerOperation(Summary = "Description: This endpoint allows users to get the list of all books")]
        [HttpGet("GetBooks")]
        [ProducesResponseType(typeof(APIResponse<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersAsync()
        {
            // Call GetUsersAsync methode from _userService
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




    }
}
