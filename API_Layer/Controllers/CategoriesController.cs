using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.CategoryDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}", Name = "GetCategoryByID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCategoryByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                var category = await _categoryService.GetCategoryByIDAsync(id);
                return Ok(category);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.NotFound => NotFound(ex.Message),
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryResponse>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCategoryRequest))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewCategoryAsync(CreateCategoryRequest category)
        {
            if (category == null || !category.IsValid())
                return BadRequest("Invalid category data");

            try
            {
                int? id = await _categoryService.AddNewCategoryAsync(category);
                if (id == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create category");

                category.SetID(id.Value);
                return CreatedAtRoute("GetCategoryByID", new { id = id.Value }, category);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    ActionResultEnum.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpPut("{id}/update", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCategoryAsync(int id, UpdateCategoryRequest category)
        {
            if (id <= 0 || category == null)
                return BadRequest("Invalid data");

            try
            {
                bool updated = await _categoryService.UpdateCategoryAsync(id, category);
                if (updated)
                    return Ok("Category updated successfully");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update category");
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.NotFound => NotFound(ex.Message),
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpDelete("{id}/delete", Name = "DeleteCategoryByID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCategoryByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                bool deleted = await _categoryService.DeleteCategoryByIDAsync(id);
                if (deleted)
                    return NoContent();
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete category");
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.NotFound => NotFound(ex.Message),
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }
    }
}