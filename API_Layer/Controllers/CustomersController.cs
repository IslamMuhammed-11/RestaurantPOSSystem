using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.CustomerDTOs;
using Contracts.Enums;
using Microsoft.AspNetCore.Authorization;

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
            if (id <= 0)
                return BadRequest("Invalid ID");

            var customer = await _customerService.GetCustomerByIDAsync(id);
            if (customer == null)
                return NotFound("Customer not found");

            return Ok(customer);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCustomersAsync()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateCustomerRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewCustomerAsync(CreateCustomerRequest customer)
        {
            if (customer == null || string.IsNullOrEmpty(customer.Name))
                return BadRequest("Invalid customer data");

            int? ID = await _customerService.AddNewCustomerAsync(customer);
            if (ID == null)
                return StatusCode(500);

            customer.SetID(ID.Value);

            return CreatedAtRoute("GetCustomerByID", new { id = ID }, customer);
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
            return result switch
            {
                ActionResultEnum.ActionResult.NotFound => NotFound("Customer not found"),
                ActionResultEnum.ActionResult.InvalidData => BadRequest("Invalid customer data"),
                ActionResultEnum.ActionResult.Success => Ok("Customer updated successfully"),
                _ => StatusCode(500)
            };
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
            return result switch
            {
                ActionResultEnum.ActionResult.NotFound => NotFound("Customer not found"),
                ActionResultEnum.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}