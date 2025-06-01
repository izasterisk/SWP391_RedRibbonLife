using System.Net;
using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UserController(//ILogger<UserController> logger, IMapper mapper, 
            IUserService userService)
        {
            //_mapper = mapper;
            _userService = userService;
        }
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager")]
        public async Task<ActionResult<APIResponse>> CreateUserAsync(UserDTO dto)
        {
            var apiResponse = new APIResponse();
            try
            {
                var userCreated = await _userService.CreateUserAsync(dto);
                apiResponse.Data = userCreated;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                //Ok - 200
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return apiResponse;
            }
        }
        [HttpGet]
        [Route("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetAllUsersAsync()
        {
            var apiResponse = new APIResponse();
            try
            {
                var users = await _userService.GetAllUserAsync();
                apiResponse.Data = users;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                //Ok - 200
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return apiResponse;
            }
        }
        [HttpGet]
        [Route("{fullname}", Name = "GetUserByFullName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetUserByUsernameAsync(string fullname)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(fullname))
                {
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    apiResponse.Errors.Add("Invalid username provided.");
                    return BadRequest(apiResponse);
                }
                var user = await _userService.GetUserByFullnameAsync(fullname);
                if (user == null)
                {
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    apiResponse.Errors.Add($"User with username {fullname} not found.");
                    return NotFound(apiResponse);
                }
                apiResponse.Data = user;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                //Ok - 200
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return apiResponse;
            }
        }
        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager")]
        public async Task<ActionResult<APIResponse>> UpdateUserAsync(UserDTO dto)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (dto == null || dto.UserId <= 0)
                {
                    return BadRequest(new APIResponse
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = { "Invalid user data provided." }
                    });
                }
                var result = await _userService.UpdateUserAsync(dto);
                apiResponse.Data = result;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                //Ok - 200
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return apiResponse;
            }
        }
        //Chua lam delete tai khong biet co nen lam soft delete hay khong???
    }
}
