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
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                var product = await _productService.GetProductByIDAsync(id);
                return Ok(product);
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
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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
            if (product == null || !product.IsValid())
                return BadRequest("Invalid product data");

            try
            {
                int? id = await _productService.AddNewProductAsync(product);
                if (id == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create product");

                product.SetID(id.Value);
                return CreatedAtRoute("GetProductByID", new { id = id.Value }, product);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    ActionResultEnum.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
                    ActionResultEnum.ActionResult.NotFound => NotFound(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
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
            if (id <= 0 || product == null)
                return BadRequest("Invalid data");

            try
            {
                bool updated = await _productService.UpdateProductAsync(id, product);
                if (updated)
                    return Ok("Product updated successfully");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update product");
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
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                bool deleted = await _productService.DeleteProductByIDAsync(id);
                if (deleted)
                    return NoContent();
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete product");
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