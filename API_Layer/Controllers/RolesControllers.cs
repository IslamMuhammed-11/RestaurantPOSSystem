using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.RolesDTOs;
using Contracts.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/roles")]
    [ApiController]
    public class RolesControllers : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesControllers(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet("All Roles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RoleResponse>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var roles = await _rolesService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}", Name = "GetRoleByID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoleByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var role = await _rolesService.GetRoleByIDAsync(id);
            if (role == null)
                return NotFound("Role not found");

            return Ok(role);
        }

        [HttpPost("Add New Role")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateRoleRequest))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewRoleAsync(CreateRoleRequest role)
        {
            if (role == null || !role.IsValid())
                return BadRequest("Invalid role data");

            int? ID = await _rolesService.AddNewRoleAsync(role);
            if (ID == null)
                return StatusCode(500);

            role.SetID(ID.Value);

            return CreatedAtAction("GetRoleByID", new { id = ID }, role);
        }

        [HttpPut("Update/{id}", Name = "UpdateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRoleAsync(int id, UpdateRoleRequest role)
        {
            if (id <= 0 || role == null)
                return BadRequest("Invalid data");

            var result = await _rolesService.UpdateRoleAsync(id, role);

            return result switch
            {
                Enums.ActionResult.NotFound => NotFound("Role not found"),
                Enums.ActionResult.InvalidData => BadRequest("Invalid role data"),
                Enums.ActionResult.Success => Ok("Role updated successfully"),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("Delete/{id}", Name = "DeleteRoleByID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRoleByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var result = await _rolesService.DeleteRoleByIDAsync(id);

            return result switch
            {
                Enums.ActionResult.NotFound => NotFound("Role not found"),
                Enums.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}