using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.ProductDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Route("api/ProductsController")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}", Name = "GetProductByID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReadProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpGet("All Products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        [HttpPost("Add New Product")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddNewProductAsync(CreateProductDTO product)
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
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpPut("Update/{id}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProductAsync(int id, UpdateProductDTO product)
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
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpDelete("Delete/{id}", Name = "DeleteProductByID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }
    }
}
