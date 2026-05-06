using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.CustomerDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}", Name = "GetCustomerByID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCustomerByIDAsync(int id)
        {
            var result = await _customerService.GetCustomerByIDAsync(id);

            return Mapping.ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var result = await _customerService.GetAllCustomersAsync();
            return Mapping.ResultMappingExtensions.ToActionResult(result);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCustomerRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewCustomerAsync(CreateCustomerRequest customer)
        {
            var result = await _customerService.AddNewCustomerAsync(customer);

            if (!result.IsSuccess)
                return Mapping.ResultMappingExtensions.ToActionResult(result);

            return CreatedAtRoute("GetCustomerByID", new { id = result.Value.CustomerID }, result.Value);
        }

        [HttpPut("{id}/update", Name = "UpdateCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCustomerAsync(int id, UpdateCustomerRequest customer)
        {
            if (id <= 0 || customer == null || string.IsNullOrEmpty(customer.Name))
                return BadRequest("Invalid data");
            var result = await _customerService.UpdateCustomerAsync(id, customer);

            return ResultMappingExtensions.ToActionResult(result, "Updated successfully");
        }

        [HttpDelete("{id}/delete", Name = "DeleteCustomerByID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCustomerByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");
            var result = await _customerService.DeleteCustomerByIDAsync(id);

            if (!result.IsSuccess)
                return Mapping.ResultMappingExtensions.ToActionResult(result);

            return NoContent();
        }
    }
}