using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.PaymentMethodDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Contracts.DTOs.BaseResponse;

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
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentMethodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaymentMethodByIDAsync(int id)
        {
            var method = await _service.GetMethodByIdAsync(id);

            return ResultMappingExtensions.ToActionResult(method);
        }

        [HttpGet(Name = "GetAllPaymentMethods")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMethodsAsync()
        {
            var methods = await _service.GetAllMethodsAsync();

            return ResultMappingExtensions.ToActionResult(methods);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PaymentMethodResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewMethodAsync([FromBody] CreatePaymentMethodRequest dto)
        {
            var result = await _service.AddNewMethodAsync(dto);

            if (!result.IsSuccess)
                return ResultMappingExtensions.ToActionResult(result);

            return CreatedAtRoute("GetPaymentMethodByID", new { id = result.Value.MethodID }, ApiResponse<PaymentMethodResponse>.Success(result.Value));
        }

        [HttpPut("{id}", Name = "UpdatePaymentMethod")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMethodAsync(int id, [FromBody] UpdatePaymentMethodRequest dto)
        {
            var result = await _service.UpdateMethodAsync(id, dto);

            return ResultMappingExtensions.ToActionResult(result, "Updated Successfully");
        }

        [HttpDelete("{id}", Name = "DeletePaymentMethodByID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMethodAsync(int id)
        {
            var result = await _service.DeleteMethodAsync(id);

            return ResultMappingExtensions.ToActionResult(result, null, false);
        }
    }
}