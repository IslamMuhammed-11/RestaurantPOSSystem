using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.PaymentDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            if (orderId <= 0)
                return BadRequest("ID must be non negative number");

            try
            {
                var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
                return Ok(payment);
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

        [HttpGet(Name = "GetAllPayments")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync();
                return Ok(payments);
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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
            if (payment == null || !payment.IsValid())
                return BadRequest("Invalid payment data");

            try
            {
                int? id = await _paymentService.CreateNewPaymentAsync(orderId, payment);
                if (!id.HasValue)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create payment");

                var created = await _paymentService.GetPaymentByPaymentIdAsync(id.Value);

                var response = new CreatePaymentCreatedResponse
                {
                    PaymentID = id.Value,
                    OrderID = created?.OrderID ?? orderId,
                    PaidAmount = created?.PaymentAmount ?? payment.PaymentAmount
                };

                return CreatedAtRoute("GetPaymentByOrderID", new { orderId = response.OrderID }, response);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.Conflict => Conflict(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }
    }
}