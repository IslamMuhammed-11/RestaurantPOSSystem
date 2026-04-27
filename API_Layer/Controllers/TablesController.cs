using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.TableDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/TablesController")]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTableByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                var table = await _tableService.GetTableByIDAsync(id);
                return Ok(table);
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

        [HttpGet("All Tables")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TableResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTablesAsync()
        {
            try
            {
                var tables = await _tableService.GetAllTablesAsync();
                return Ok(tables);
            }
            catch (BusinessException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Add New Table")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateTableRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewTableAsync(CreateTableRequest table)
        {
            if (table == null || !table.IsValid())
                return BadRequest("Invalid table data");

            try
            {
                int? id = await _tableService.AddNewTableAsync(table);
                if (id == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create table");

                table.SetID(id.Value);
                return CreatedAtRoute("GetTableByID", new { id = id.Value }, table);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpPut("Update/{id}", Name = "UpdateTable")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        private async Task<IActionResult> UpdateTableAsync(int id, UpdateTableRequest table)
        {
            if (id <= 0 || table == null)
                return BadRequest("Invalid data");

            try
            {
                bool updated = await _tableService.UpdateTableAsync(id, table);
                if (updated)
                    return Ok("Table updated successfully");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update table");
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

        [HttpDelete("Delete/{id}", Name = "DeleteTableByID")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTableByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            try
            {
                bool deleted = await _tableService.DeleteTableByIDAsync(id);
                if (deleted)
                    return NoContent();
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete table");
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