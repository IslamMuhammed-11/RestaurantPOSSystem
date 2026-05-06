using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.BaseResponse;
using Contracts.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Authorization;
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

            var category = await _categoryService.GetCategoryByIDAsync(id);

            return Mapping.ResultMappingExtensions.ToActionResult(category);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryResponse>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            return Mapping.ResultMappingExtensions.ToActionResult(categories);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCategoryRequest))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewCategoryAsync(CreateCategoryRequest category)
        {
            var result = await _categoryService.AddNewCategoryAsync(category);

            if (!result.IsSuccess)
                return Mapping.ResultMappingExtensions.ToActionResult(result);

            return CreatedAtRoute("GetCategoryByID", new { id = result.Value }, ApiResponse<CategoryResponse>.Success(result.Value));
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
            var result = await _categoryService.UpdateCategoryAsync(id, category);

            return Mapping.ResultMappingExtensions.ToActionResult(result, "Updated successfully");
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
            var result = await _categoryService.DeleteCategoryByIDAsync(id);

            return Mapping.ResultMappingExtensions.ToActionResult(result, null, false);
        }
    }
}