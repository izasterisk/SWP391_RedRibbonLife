﻿using System.IdentityModel.Tokens.Jwt;
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
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;

        public LoginController(IConfiguration configuration, ILoginService loginService, IUserService userService)
        {
            _configuration = configuration;
            _loginService = loginService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please enter username and password");
            }

            // Validate user từ database
            var user = await _loginService.ValidateUserAsync(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            LoginResponseDTO response = new()
            {
                Username = user.Username
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
                    //Roles
                    new Claim(ClaimTypes.Role, user.UserRole)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            //Generate Token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            response.token = tokenHandler.WriteToken(token);

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
        public ActionResult Logout()
        {
            try
            {
                // Lấy thông tin user từ token claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Log thông tin logout (có thể ghi vào database hoặc log file)
                // TODO: Có thể implement token blacklist ở đây nếu cần

                return Ok(new
                {
                    message = "Logout successful",
                    username = username,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred during logout",
                    error = ex.Message
                });
            }
        }

        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser")]
        public ActionResult GetCurrentUser()
        {
            try
            {
                // Lấy thông tin user từ token claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Invalid token");
                }

                return Ok(new
                {
                    userId = userId,
                    username = username,
                    userRole = userRole,
                    tokenValid = true,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while getting user info",
                    error = ex.Message
                });
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Register(UserDTO model)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    apiResponse.Status = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Errors.AddRange(ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(apiResponse);
                }

                var result = await _userService.CreateUserAsync(model);

                apiResponse.Data = new
                {
                    message = "User registered successfully",
                    username = model.Username,
                    email = model.Email
                };
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.Created;

                return StatusCode(201, apiResponse);
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
