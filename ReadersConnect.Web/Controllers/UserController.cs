using Microsoft.AspNetCore.Mvc;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Services.Implementations;
using ReadersConnect.Application.Services.Interfaces;
using ReadersConnect.Web.Extensions.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace ReadersConnect.Web.Controllers
{
    [Route("api/v1/Users")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows super admin and librarian to create library staff account")]
        [Authorize("SuperAdmin", "Librarian")]
        [HttpPost("RegisterStaff")]
        [ProducesResponseType(typeof(APIResponse<StaffRegistrationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterStaffAsync([FromBody] RegisterStaffRequestDTO requestDTO)
        {
            var result = await _userService.RegisterStaffAsync(requestDTO);

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

        [SwaggerOperation(Summary = "Description: This endpoint allows library staff to get the list of all registered users")]
        [Authorize("SuperAdmin", "Librarian", "LibrarianAssistant")]
        [HttpGet("GetUsers")]
        [ProducesResponseType(typeof(APIResponse<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersAsync()
        {
            // Call GetUsersAsync methode from _userService
            var result = await _userService.GetUsersAsync();

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

        [SwaggerOperation(Summary = "Description: This endpoint allows super admin and library manager to add roles to database")]
        [Authorize("SuperAdmin", "Librarian")]
        [HttpPost("AddRole")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddRoleAsync(CreateRoleRequestDto requestDto)
        {
            var result = await _userService.AddRoleAsync(requestDto);

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

        [SwaggerOperation(Summary = "Description: This endpoint allows super admin and library manager to add permission to database")]
        [Authorize("SuperAdmin", "Librarian")]
        [HttpPost("AddPermission")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddPermissionAsync(CreatePermissionRequestDto requestDto)
        {
            var result = await _userService.AddPermissionAsync(requestDto);

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

        [SwaggerOperation(Summary = "Description: This endpoint allows super admin and library manager to delete permission")]
        [Authorize("SuperAdmin", "Librarian")]
        [HttpDelete("permission/{permissionId}")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePermissionAsync(int permissionId)
        {
            var result = await _userService.DeletePermissionAsync(permissionId);

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

        [SwaggerOperation(Summary = "Description: This endpoint allows admin and libraian to assign role to a user")]
        [Authorize("SuperAdmin", "Librarian")]
        [HttpPost("assign-role")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AssignRoleToUserAsync([FromBody] AssignRoleRequestDTO request)
        {
            var result = await _userService.AssignRoleToUserAsync(request);

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
                return NotFound(result);

            if (result.HttpStatusCode == HttpStatusCode.Conflict)
                return Conflict(result);

            if (result.HttpStatusCode != HttpStatusCode.OK)
                return BadRequest(result);

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to register on the platform and become a library member")]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterMembersAsync([FromBody] RegisterUserRequestDTO requestDTO)
        {
            var result = await _userService.RegisterMembersAsync(requestDTO);

            if (result.HttpStatusCode == HttpStatusCode.Conflict)
                return Conflict(result);

            if (result.HttpStatusCode != HttpStatusCode.OK)
                return BadRequest(result);

            return Ok(result);
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to update their information")]
        [Authorize("SuperAdmin", "Librarian", "LibrarianAssistant", "Member")]
        [HttpPut("users/{userId}")]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoDataAPIResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditUserAsync(string userId, [FromBody] EditUserRequestDTO requestDTO)
        {
            var result = await _userService.EditUserAsync(userId, requestDTO);

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
