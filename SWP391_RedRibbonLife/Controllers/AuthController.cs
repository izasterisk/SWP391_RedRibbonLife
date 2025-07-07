using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
                var user = await _loginService.ValidateUserAsync(model.Username, model.Password);
                if (user == null)
                {
                    apiResponse.Status = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Errors.Add("Invalid username or password");
                    return BadRequest(apiResponse);
                }
                LoginResponseDTO response = new()
                {
                    Username = user.Username,
                    FullName = user.FullName
                };
                string audience = _configuration.GetValue<string>("LocalAudience");
                string issuer = _configuration.GetValue<string>("LocalIssuer");
                byte[] key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforLocaluser"));
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Issuer = issuer,
                    Audience = audience,
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        //UserId
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        //Username
                        new Claim(ClaimTypes.Name, user.Username),
                        //FullName
                        new Claim(ClaimTypes.GivenName, user.FullName ?? ""),
                        //Roles
                        new Claim(ClaimTypes.Role, user.UserRole)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
                };
                //Generate Token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.token = tokenHandler.WriteToken(token);
                apiResponse.Data = response;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
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
        public ActionResult<APIResponse> GetCurrentUserAsync()
        {
            var apiResponse = new APIResponse();
            try
            {
                // Lấy thông tin user từ token claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var fullName = User.FindFirst(ClaimTypes.GivenName)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (string.IsNullOrEmpty(username))
                {
                    apiResponse.Status = false;
                    apiResponse.StatusCode = HttpStatusCode.Unauthorized;
                    apiResponse.Errors.Add("Invalid token");
                    return Unauthorized(apiResponse);
                }

                var userData = new
                {
                    userId = userId,
                    username = username,
                    fullName = fullName,
                    userRole = userRole,
                    tokenValid = true,
                    timestamp = DateTime.UtcNow
                };

                apiResponse.Data = userData;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
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
