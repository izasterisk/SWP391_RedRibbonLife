using System.Net;
using AutoMapper;
using BLL.DTO;
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
    }
}
