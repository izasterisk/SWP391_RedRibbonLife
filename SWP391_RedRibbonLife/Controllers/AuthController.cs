using System.Security.Claims;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BLL.DTO.Login;
using BLL.DTO.User;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, ILoginService loginService, IUserService userService)
        {
            _configuration = configuration;
            _loginService = loginService;
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> LoginAsync(LoginDTO model)
        {
            var apiResponse = new APIResponse();
            if (!ModelState.IsValid)
            {
                apiResponse.Status = false;
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.Errors.AddRange(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(apiResponse);
            }
            try
            {
                var result = await _loginService.LoginServiceAsync(model);
                apiResponse.Data = result;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid username or password"))
                {
                    apiResponse.Status = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Errors.Add(ex.Message);
                    return BadRequest(apiResponse);
                }
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet]
        [Route("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
        public async Task<ActionResult<APIResponse>> GetCurrentUserAsync()
        {
            var apiResponse = new APIResponse();
            try
            {
                // Lấy userId từ token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    apiResponse.Status = false;
                    apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    apiResponse.Errors.Add("Invalid token");
                    return Unauthorized(apiResponse);
                }
                var result = await _loginService.GetMeAsync(userId);
                apiResponse.Data = result;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("User not found") || ex.Message.Contains("deactivated"))
                {
                    apiResponse.Errors.Add(ex.Message);
                    apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    apiResponse.Status = false;
                    return Unauthorized(apiResponse);
                }
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }

        [HttpPut]
        [Route("UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
        public async Task<ActionResult<APIResponse>> UpdatePasswordAsync(ChangePasswordDTO dto)
        {
            var apiResponse = new APIResponse();
            if (!ModelState.IsValid)
            {
                apiResponse.Status = false;
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.Errors.AddRange(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(apiResponse);
            }
            try
            {
                var result = await _loginService.ChangePasswordAsync(dto);
                apiResponse.Data = result;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                apiResponse.Status = false;
                return Unauthorized(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }
    }
}
