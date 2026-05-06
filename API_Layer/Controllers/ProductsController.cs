using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.ProductDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}", Name = "GetProductByID")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProductByIDAsync(int id)
        {
            var result = await _productService.GetProductByIDAsync(id);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet()]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var result = await _productService.GetAllProductsAsync();

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpPost()]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewProductAsync(CreateProductRequest product)
        {
            var result = await _productService.AddNewProductAsync(product);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpPut("{id}", Name = "UpdateProduct")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProductAsync(int id, UpdateProductRequest product)
        {
            var result = await _productService.UpdateProductAsync(id, product);

            return ResultMappingExtensions.ToActionResult(result, "Updated Successfully");
        }

        [HttpDelete("{id}", Name = "DeleteProductByID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProductByIDAsync(int id)
        {
            var result = await _productService.DeleteProductByIDAsync(id);

            return ResultMappingExtensions.ToActionResult(result, "Deleted Successfully", false);
        }
    }
}