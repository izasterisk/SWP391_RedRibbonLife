using System.Net;
using BLL.DTO;
using BLL.DTO.ARVComponent;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ARVComponentsController : ControllerBase
    {
        private readonly IARVComponentService _arvComponentService;

        public ARVComponentsController(IARVComponentService arvComponentService)
        {
            _arvComponentService = arvComponentService;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff, Doctor")]
        public async Task<ActionResult<APIResponse>> CreateARVComponentAsync(ARVComponentCreateDTO dto)
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
                var arvComponentCreated = await _arvComponentService.CreateARVComponentAsync(dto);
                apiResponse.Data = arvComponentCreated;
                apiResponse.Status = true;
                apiResponse.StatusCode = HttpStatusCode.Created;
                var componentId = arvComponentCreated.ComponentId;
                return Created($"api/ARVComponents/GetByID/{componentId}", apiResponse);
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
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff, Doctor")]
        public async Task<ActionResult<APIResponse>> UpdateARVComponentAsync(ARVComponentUpdateDTO dto)
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
                var arvComponentUpdated = await _arvComponentService.UpdateARVComponentAsync(dto);
                apiResponse.Data = arvComponentUpdated;
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
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff, Doctor")]
        public async Task<ActionResult<APIResponse>> GetAllARVComponentsAsync()
        {
            var apiResponse = new APIResponse();
            try
            {
                var arvComponents = await _arvComponentService.GetAllARVComponentAsync();
                apiResponse.Data = arvComponents;
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
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff, Doctor")]
        public async Task<ActionResult<APIResponse>> GetARVComponentByIdAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("ARVComponent ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                var arvComponent = await _arvComponentService.GetARVComponentByIdAsync(id);
                apiResponse.Data = arvComponent;
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

        [HttpDelete]
        [Route("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff, Doctor")]
        public async Task<ActionResult<APIResponse>> DeleteARVComponentByIdAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("ARVComponent ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                var result = await _arvComponentService.DeleteARVComponentByIdAsync(id);
                apiResponse.Data = result;
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
    }
}