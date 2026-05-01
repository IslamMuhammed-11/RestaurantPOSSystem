using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.RefundedPaymentsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/refunds")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RefundsController : ControllerBase
    {
        private readonly IRefundedPaymentsService _refundedPaymentsService;

        public RefundsController(IRefundedPaymentsService refundedPaymentsService)
        {
            _refundedPaymentsService = refundedPaymentsService;
        }

        [HttpGet(Name = "GetAllRefundedPayments")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RefundedPaymentResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllRefundedPaymentsAsync()
        {
            try
            {
                var refunds = await _refundedPaymentsService.GetAllRefundedPaymentsAsync();
                return Ok(refunds);
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{refundId}", Name = "GetRefundedPaymentByID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefundedPaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetRefundedPaymentByIDAsync(int refundId)
        {
            if (refundId <= 0)
                return BadRequest("Invalid refund ID.");

            try
            {
                var refund = await _refundedPaymentsService.GetRefundedPaymentByIDAsync(refundId);
                return Ok(refund);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    ActionResultEnum.ActionResult.NotFound => NotFound(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RefundedPaymentResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefundPaymentAsync([FromBody] CreateRefundPaymentRequest refund)
        {
            if (refund == null || !refund.IsValid())
                return BadRequest("Invalid refund data.");

            try
            {
                int? refundId = await _refundedPaymentsService.RefundPaymentAsync(refund);
                if (!refundId.HasValue)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create refund.");

                var response = await _refundedPaymentsService.GetRefundedPaymentByIDAsync(refundId.Value);
                if (response == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to load created refund.");

                return CreatedAtRoute("GetRefundedPaymentByID", new { refundId = response.RefundID }, response);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    ActionResultEnum.ActionResult.InvalidData => BadRequest(ex.Message),
                    ActionResultEnum.ActionResult.NotFound => NotFound(ex.Message),
                    ActionResultEnum.ActionResult.Conflict => Conflict(ex.Message),
                    ActionResultEnum.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }
    }
}