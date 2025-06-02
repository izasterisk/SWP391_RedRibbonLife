using System.Net;
using AutoMapper;
using BLL.DTO;
using BLL.DTO.Doctor;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService DoctorService)
        {
            _doctorService = DoctorService;
        }
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager")]
        public async Task<ActionResult<APIResponse>> CreateDoctorAsync(DoctorDTO dto)
        {
            var apiResponse = new APIResponse();
            try
            {
                var doctorCreated = await _doctorService.CreateDoctorAsync(dto);
                apiResponse.Data = doctorCreated;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> UpdateDoctorAsync(DoctorDTO dto)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (dto.UserId <= 0)
                {
                    apiResponse.Errors.Add("User ID must be greater than 0.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }

                var doctorUpdated = await _doctorService.UpdateDoctorAsync(dto);
                apiResponse.Data = doctorUpdated;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found"))
                {
                    apiResponse.Errors.Add(ex.Message);
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.Status = false;
                    return NotFound(apiResponse);
                }

                apiResponse.Errors.Add(ex.Message);
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Status = false;
                return StatusCode(500, apiResponse);
            }
        }

        [HttpGet]
        [Route("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Doctor")]
        public async Task<ActionResult<APIResponse>> GetAllDoctorsAsync()
        {
            var apiResponse = new APIResponse();
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                apiResponse.Data = doctors;
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
    }
}
