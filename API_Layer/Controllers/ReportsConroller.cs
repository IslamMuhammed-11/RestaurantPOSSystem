using BusinessLogicLayer.Interfaces;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Contracts.Queries.ReportsQueries;
using Contracts.DTOs;

namespace API_Layer.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IDailySalesService _dailySalesService;
        private readonly IProductSalesService _productsSalesService;

        public ReportsController(IDailySalesService dailySalesService, IProductSalesService productsSalesService)
        {
            _dailySalesService = dailySalesService;
            _productsSalesService = productsSalesService;
        }

        [HttpGet("top-products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopProductsAsync([FromQuery] RangedQuery query)

        {
            try
            {
                var response = await _productsSalesService.GetTopProductsAsync(query);

                return Ok(response);
            }
            catch (BusinessException ex)
            {
                if (ex.ErrorType == ActionResultEnum.ActionResult.InvalidData)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("sales-comparison")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesComparisonAsync([FromQuery] SalesComparisonQuery query)
        {
            try
            {
                var response = await _dailySalesService.GetSalesComparisonAsync(query);

                return Ok(response);
            }
            catch (BusinessException ex)
            {
                if (ex.ErrorType == ActionResultEnum.ActionResult.InvalidData)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("sales-details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesDetails([FromQuery] RangedQuery query)
        {
            try
            {
                var response = await _dailySalesService.GetSalesDetailsAsync(query);

                return Ok(response);
            }
            catch (BusinessException ex)
            {
                if (ex.ErrorType == ActionResultEnum.ActionResult.InvalidData)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("sales-trends")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaleTrends([FromQuery] PeriodicQuery query)
        {
            try
            {
                var response = await _dailySalesService.GetSalesTrendsAsync(query);

                return Ok(response);
            }
            catch (BusinessException ex)
            {
                if (ex.ErrorType == ActionResultEnum.ActionResult.InvalidData)
                    return BadRequest(ex.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}