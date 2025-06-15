using System.Net;
using System.Linq;
using AutoMapper;
using BLL.DTO;
using BLL.DTO.Category;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_RedRibbonLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult<APIResponse>> CreateCategoryAsync(CategoryDTO dto)
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
                var categoryCreated = await _categoryService.CreateCategoryAsync(dto);
                apiResponse.Data = categoryCreated;
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
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult<APIResponse>> UpdateCategoryAsync(CategoryDTO dto)
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
                var categoryUpdated = await _categoryService.UpdateCategoryAsync(dto);
                apiResponse.Data = categoryUpdated;
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
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult<APIResponse>> GetAllCategoriesAsync()
        {
            var apiResponse = new APIResponse();
            try
            {
                var categories = await _categoryService.GetAllCategoryAsync();
                apiResponse.Data = categories;
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
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult<APIResponse>> GetCategoryByIdAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("Category ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                var category = await _categoryService.GetCategoryByIdAsync(id);
                apiResponse.Data = category;
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
        [Authorize(AuthenticationSchemes = "LoginforLocaluser", Roles = "Admin, Manager, Staff")]
        public async Task<ActionResult<APIResponse>> DeleteCategoryAsync(int id)
        {
            var apiResponse = new APIResponse();
            try
            {
                if (id <= 0)
                {
                    apiResponse.Errors.Add("Category ID must be a positive integer.");
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Status = false;
                    return BadRequest(apiResponse);
                }
                var result = await _categoryService.DeleteCategoryByIdAsync(id);
                if (result)
                {
                    apiResponse.Data = new { message = "Category deleted successfully" };
                    apiResponse.Status = true;
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.Errors.Add("Failed to delete category");
                    apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    apiResponse.Status = false;
                    return StatusCode(500, apiResponse);
                }
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
