using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.PaymentDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("payments/by-order/{orderId}", Name = "GetPaymentByOrderID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaymentByOrderIDAsync(int orderId)
        {
            var result = await _paymentService.GetPaymentByOrderIdAsync(orderId);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet(Name = "GetAllPayments")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllPaymentsAsync()
        {
            var result = await _paymentService.GetAllPaymentsAsync();

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpPost("orders/{orderId}/payments")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePaymentCreatedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatePaymentAsync(int orderId, [FromBody] CreatePaymentRequest payment)
        {
            var result = await _paymentService.CreateNewPaymentAsync(orderId, payment);

            if (!result.IsSuccess)
                return ResultMappingExtensions.ToActionResult(result);

            return CreatedAtRoute("GetPaymentByOrderID", new { orderId = result.Value.OrderID }, result.Value);
        }
    }
}