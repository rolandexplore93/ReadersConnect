using Microsoft.AspNetCore.Mvc;
using ReadersConnect.Application.DTOs.Requests;
using ReadersConnect.Application.DTOs.Responses;
using ReadersConnect.Application.Services.Implementations;
using ReadersConnect.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace ReadersConnect.Web.Controllers
{
    [Route("api/v1/Users")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        public UserController(IAuthService authService)
        {
            //_authService = authService;
        }

        [SwaggerOperation(Summary = "Description: This endpoint allows users to login")]
        [HttpPost("RegisterStaff")]
        [ProducesResponseType(typeof(APIResponse<LoginTokenDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDTO requestDTO)
        {
            // Call LoginAsync method from AuthService
            var result = await _authService.LoginAsync(requestDTO);

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
