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
                var result = await _emailService.SendVerificationEmailAsync(request.Email);
                
                if (result)
                {
                    apiResponse.Data = new
                    {
                        message = "Verification email sent successfully",
                        email = request.Email
                    };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Unable to send email");
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
                apiResponse.Errors.Add($"Error sending email: {ex.Message}");
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
                        message = "Account verification successful",
                        email = request.Email,
                        isVerified = true
                    };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Verification failed");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid verification code"))
                {
                    apiResponse.Errors.Add("Invalid verification code");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                if (ex.Message.Contains("not found"))
                {
                    apiResponse.Errors.Add("User not found");
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    return NotFound(apiResponse);
                }
                apiResponse.Errors.Add($"Error during verification: {ex.Message}");
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }

        [HttpPost]
        [Route("SendForgotPasswordEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SendForgotPasswordEmailAsync([FromBody] ForgotPasswordRequestDTO request)
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
                var result = await _emailService.SendForgotPasswordEmailAsync(request.Email);
                
                if (result)
                {
                    apiResponse.Data = new
                    {
                        message = "Password reset email sent successfully",
                        email = request.Email
                    };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Unable to send password reset email");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    apiResponse.Errors.Add("Account not found");
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    return NotFound(apiResponse);
                }
                apiResponse.Errors.Add($"Error sending password reset email: {ex.Message}");
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ResetPasswordAsync([FromBody] ResetPasswordDTO request)
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
                var result = await _emailService.ChangePatientPasswordAsync(request.Email, request.VerifyCode, request.NewPassword);
                if (result)
                {
                    apiResponse.Data = new
                    {
                        message = "Password reset successful",
                        email = request.Email
                    };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Password reset failed");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid verification code"))
                {
                    apiResponse.Errors.Add("Invalid or expired verification code");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                if (ex.Message.Contains("not found"))
                {
                    apiResponse.Errors.Add("User not found");
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    return NotFound(apiResponse);
                }
                apiResponse.Errors.Add($"Error resetting password: {ex.Message}");
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }
    }
} 