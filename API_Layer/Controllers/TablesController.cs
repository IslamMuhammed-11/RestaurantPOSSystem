using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.TableDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/tables")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet("{id}", Name = "GetTableByID")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTableByIDAsync(int id)
        {
            var result = await _tableService.GetTableByIDAsync(id);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet()]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TableResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTablesAsync()
        {
            var result = await _tableService.GetAllTablesAsync();

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpPost()]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateTableRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewTableAsync(CreateTableRequest table)
        {
            var result = await _tableService.AddNewTableAsync(table);

            if (!result.IsSuccess)
                return ResultMappingExtensions.ToActionResult(result);

            return CreatedAtRoute("GetTableByID", new { id = result.Value.TableID }, result.Value);
        }

        [HttpPut("{id}", Name = "UpdateTable")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        private async Task<IActionResult> UpdateTableAsync(int id, UpdateTableRequest table)
        {
            var result = await _tableService.UpdateTableAsync(id, table);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpDelete("{id}", Name = "DeleteTableByID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTableByIDAsync(int id)
        {
            var result = await _tableService.DeleteTableByIDAsync(id);

            return ResultMappingExtensions.ToActionResult(result);
        }
    }
}