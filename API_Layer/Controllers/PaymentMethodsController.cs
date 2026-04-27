using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.PaymentMethodDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/payment-methods")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodService _service;

        public PaymentMethodsController(IPaymentMethodService service)
        {
            _service = service;
        }

        [HttpGet("{id}", Name = "GetPaymentMethodByID")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentMethodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaymentMethodByIDAsync(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            try
            {
                var method = await _service.GetMethodByIdAsync(id);
                return Ok(method);
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

        [HttpGet(Name = "GetAllPaymentMethods")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMethodsAsync()
        {
            try
            {
                var methods = await _service.GetAllMethodsAsync();
                return Ok(methods);
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PaymentMethodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewMethodAsync([FromBody] CreatePaymentMethodRequest dto)
        {
            if (dto == null || !dto.IsValid())
                return BadRequest("Invalid payment method data");

            try
            {
                int? id = await _service.AddNewMethodAsync(dto);
                if (!id.HasValue)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create payment method");

                var created = await _service.GetMethodByIdAsync(id.Value);
                return CreatedAtRoute("GetPaymentMethodByID", new { id = id.Value }, created);
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

        [HttpPut("{id}", Name = "UpdatePaymentMethod")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMethodAsync(int id, [FromBody] UpdatePaymentMethodRequest dto)
        {
            if (id <= 0 || dto == null || !dto.IsValid())
                return BadRequest("Invalid data");

            try
            {
                bool updated = await _service.UpdateMethodAsync(id, dto);
                if (updated)
                    return Ok("Payment method updated successfully");

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update payment method");
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

        [HttpDelete("{id}", Name = "DeletePaymentMethodByID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMethodAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                bool deleted = await _service.DeleteMethodAsync(id);
                if (deleted)
                    return NoContent();

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete payment method");
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