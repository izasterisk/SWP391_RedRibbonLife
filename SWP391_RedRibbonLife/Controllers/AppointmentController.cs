using System.Net;
using BLL.DTO;
using BLL.DTO.Appointment;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Patient, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> CreateAppointmentAsync(AppointmentCreateDTO dto)
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
                var appointmentCreated = await _appointmentService.CreateAppointmentAsync(dto);
                apiResponse.Data = appointmentCreated;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.Created;
                var appointmentId = appointmentCreated.AppointmentId;
                return Created($"api/Appointment/GetByID/{appointmentId}", apiResponse);
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
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Patient, Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> UpdateAppointmentAsync(AppointmentUpdateDTO dto)
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
                var appointmentUpdated = await _appointmentService.UpdateAppointmentAsync(dto);
                apiResponse.Data = appointmentUpdated;
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
        [Route("GetByPatientId/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Patient, Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetAppointmentsByPatientIdAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("Patient ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }

                var appointments = await _appointmentService.GetAllAppointmentsByPatientIdAsync(id);
                apiResponse.Data = appointments;
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
        [Route("GetByDoctorId/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetAppointmentsByDoctorIdAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("Doctor ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }

                var appointments = await _appointmentService.GetAllAppointmentsByDoctorIdAsync(id);
                apiResponse.Data = appointments;
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
        [Route("GetByID/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Patient, Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetAppointmentByIdAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("Appointment ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                apiResponse.Data = appointment;
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
        [Route("GetAvailableDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Patient, Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetAvailableDoctorsAsync([FromQuery] DateOnly appointmentDate, [FromQuery] TimeOnly appointmentTime)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (appointmentDate == default)
                {
                    apiResponse.Errors.Add("Appointment date is required.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }

                if (appointmentTime == default)
                {
                    apiResponse.Errors.Add("Appointment time is required.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }

                var availableDoctors = await _appointmentService.GetAvailableDoctorsAsync(appointmentDate, appointmentTime);
                apiResponse.Data = availableDoctors;
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
        [Route("GetAllScheduled")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Doctor, Admin, Manager")]
        public async Task<ActionResult<APIResponse>> GetAllScheduledAppointmentsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (page < 1)
                {
                    apiResponse.Errors.Add("Page number must be greater than 0.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                
                if (pageSize < 1 || pageSize > 100)
                {
                    apiResponse.Errors.Add("Page size must be between 1 and 100.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }

                var pagedAppointments = await _appointmentService.GetAllScheduledAppointmentsAsync(page, pageSize);
                apiResponse.Data = pagedAppointments;
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

        // [HttpDelete]
        // [Route("Delete/{id}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        // [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Patient, Doctor, Admin, Manager")]
        // public async Task<ActionResult<APIResponse>> DeleteAppointmentAsync(int id)
        // {
        //     var apiResponse = new APIResponse();
        //     try
        //     {
        //         if (id <= 0)
        //         {
        //             apiResponse.Errors.Add("Appointment ID must be a positive integer.");
        //             apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //             apiResponse.Status = false;
        //             return BadRequest(apiResponse);
        //         }

        //         var result = await _appointmentService.DeleteAppointmentByIdAsync(id);
        //         if (result)
        //         {
        //             apiResponse.Data = "Appointment deleted successfully.";
        //             apiResponse.Status = true;
        //             apiResponse.StatusCode = HttpStatusCode.OK;
        //             return Ok(apiResponse);
        //         }
        //         else
        //         {
        //             apiResponse.Errors.Add("Failed to delete appointment.");
        //             apiResponse.StatusCode = HttpStatusCode.InternalServerError;
        //             apiResponse.Status = false;
        //             return StatusCode(500, apiResponse);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         if (ex.Message.Contains("not found"))
        //         {
        //             apiResponse.Errors.Add(ex.Message);
        //             apiResponse.StatusCode = HttpStatusCode.NotFound;
        //             apiResponse.Status = false;
        //             return NotFound(apiResponse);
        //         }

        //         apiResponse.Errors.Add(ex.Message);
        //         apiResponse.StatusCode = HttpStatusCode.InternalServerError;
        //         apiResponse.Status = false;
        //         return StatusCode(500, apiResponse);
        //     }
        // }
    }
}