using System.Net;
using BLL.DTO;
using BLL.DTO.Email;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [Route("SendEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SendEmailAsync([FromBody] RequestDTO request)
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
                var result = await _emailService.SendEmailAsync(request.Email);
                
                if (result)
                {
                    apiResponse.Data = new
                    {
                        message = "Email xác thực đã được gửi thành công",
                        email = request.Email
                    };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Không thể gửi email");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found") || ex.Message.Contains("already verified"))
                {
                    apiResponse.Errors.Add(ex.Message);
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    return NotFound(apiResponse);
                }

                apiResponse.Errors.Add($"Lỗi khi gửi email: {ex.Message}");
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }

        [HttpPost]
        [Route("VerifyPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> VerifyPatientAsync([FromBody] ResponseDTO request)
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
                var result = await _emailService.VerifyPatientAsync(request.Email, request.VerifyCode);
                
                if (result)
                {
                    apiResponse.Data = new
                    {
                        message = "Xác thực tài khoản thành công",
                        email = request.Email,
                        isVerified = true
                    };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Xác thực không thành công");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid verification code"))
                {
                    apiResponse.Errors.Add("Mã xác thực không hợp lệ");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                
                if (ex.Message.Contains("not found"))
                {
                    apiResponse.Errors.Add("Không tìm thấy người dùng");
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    return NotFound(apiResponse);
                }

                apiResponse.Errors.Add($"Lỗi khi xác thực: {ex.Message}");
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }
    }
} 