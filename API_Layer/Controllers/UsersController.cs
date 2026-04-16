using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.UserDTOs;
using Contracts.Enums;
using Contracts.DTOs.PersonDTOs;
namespace API_Layer.Controllers
{
    [Route("api/UsersController")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPersonService _personService;
        public UsersController(IUserService userService , IPersonService personService)
        {
            _userService = userService;
            _personService = personService;
        }

        [HttpGet("{id}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByIDAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var user = await _userService.GetUserByIDAsync(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpGet("All Users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("Add New User")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewUserAsync(CreateUserDTO user)
        {
            if (!user.IsValid())
                return BadRequest("Invalid user data");


            int? ID = await _userService.AddNewUserAsync(user);

            if (ID == null)
                return StatusCode(500);

            user.SetUserID(ID.Value);

            return CreatedAtRoute("GetUserByID", new { id = ID }, user);

        }

        [HttpPut("{id}/Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(int id, UpdateUserDTO user)
        {
            var result = await _userService.UpdateUserAsync(id, user);

            return result switch
            {
                Enums.ActionResult.InvalidData => BadRequest("Invalid data"),
                Enums.ActionResult.NotFound => NotFound("User not found"),
                Enums.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}/Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserByIDAsync(int id)
        {
            var result = await _userService.DeleteUserByIDAsync(id);
            return result switch
            {
                Enums.ActionResult.InvalidData => BadRequest("Invalid data"),
                Enums.ActionResult.NotFound => NotFound("User not found"),
                Enums.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };

        }

        [HttpPatch("{id}/Deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUserAsync(int id)
        {
            var result = await _userService.DeactivateUserAsync(id);
            return result switch
            {
                Enums.ActionResult.InvalidData => BadRequest("Invalid data"),
                Enums.ActionResult.NotFound => NotFound("User not found"),
                Enums.ActionResult.AlreadyInactive => BadRequest("User is already inactive"),
                Enums.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };

        }

        [HttpPatch("{id}/Activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivateUserAsync(int id)
        {
            var result = await _userService.ActivateUserAsync(id);
            return result switch
            {
                Enums.ActionResult.InvalidData => BadRequest("Invalid data"),
                Enums.ActionResult.NotFound => NotFound("User not found"),
                Enums.ActionResult.AlreadyActive => BadRequest("User is already active"),
                Enums.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }

        [HttpPatch("{id}/UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePasswordAsync(int id, string NewPassword, string Password)
        {
            var result = await _userService.UpdatePassword(id, NewPassword, Password);

            return result switch
            {
                Enums.ActionResult.InvalidData => BadRequest("Invalid data"),
                Enums.ActionResult.NotFound => NotFound("User not found"),
                Enums.ActionResult.InvalidPassword => BadRequest("Invalid current password"),
                Enums.ActionResult.WeakPassword => BadRequest("New password does not meet strength requirements"),
                Enums.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}
