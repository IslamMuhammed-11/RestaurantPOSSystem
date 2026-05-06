using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Queries.ReportsQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Layer.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
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
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopProductsAsync([FromQuery] RangedQuery query)

        {
            var result = await _productsSalesService.GetTopProductsAsync(query);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet("sales-comparison")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesComparisonAsync([FromQuery] SalesComparisonQuery query)
        {
            var result = await _dailySalesService.GetSalesComparisonAsync(query);
            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet("sales-details")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSalesDetails([FromQuery] RangedQuery query)
        {
            var result = await _dailySalesService.GetSalesDetailsAsync(query);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet("sales-trends")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSaleTrends([FromQuery] PeriodicQuery query)
        {
            var result = await _dailySalesService.GetSalesTrendsAsync(query);

            return ResultMappingExtensions.ToActionResult(result);
        }
    }
}