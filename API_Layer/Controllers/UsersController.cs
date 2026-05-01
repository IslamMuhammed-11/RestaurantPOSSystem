using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using Contracts.DTOs.PersonDTOs;
using Contracts.DTOs.UserDTOs;
using Contracts.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPersonService _personService;

        public UsersController(IUserService userService, IPersonService personService)
        {
            _userService = userService;
            _personService = personService;
        }

        //Ownership policy
        [HttpGet("{id}", Name = "GetUserByID")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByIDAsync(int id, [FromServices] IAuthorizationService authorizationService)
        {
            if (id <= 0)
                return BadRequest("Invalid ID");

            var user = await _userService.GetUserByIDAsync(id);

            if (user == null)
                return NotFound("User not found");

            var authResult = await authorizationService.AuthorizeAsync(User, user.UserID, "UserOwnerOrSuperOrAdmin");

            if (!authResult.Succeeded)
                return Forbid();

            return Ok(user);
        }

        [HttpGet()]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost()]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewUserAsync(CreateUserRequest user)
        {
            if (!user.IsValid())
                return BadRequest("Invalid user data");

            int? ID = await _userService.AddNewUserAsync(user);

            if (ID == null)
                return StatusCode(500);

            user.SetUserID(ID.Value);

            return CreatedAtRoute("GetUserByID", new { id = ID }, user);
        }

        //Policy Ownership
        [HttpPatch("{id}/username")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(int id, UpdateUserRequest user, [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, id, "UserOwnerOrSuperOrAdmin");

            if (!authResult.Succeeded)
                return Forbid();

            var result = await _userService.UpdateUsernameAsync(id, user);

            return result switch
            {
                ActionResultEnum.ActionResult.InvalidData => BadRequest("Invalid data"),
                ActionResultEnum.ActionResult.NotFound => NotFound("User not found"),
                ActionResultEnum.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserByIDAsync(int id)
        {
            var result = await _userService.DeleteUserByIDAsync(id);
            return result switch
            {
                ActionResultEnum.ActionResult.InvalidData => BadRequest("Invalid data"),
                ActionResultEnum.ActionResult.NotFound => NotFound("User not found"),
                ActionResultEnum.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUserAsync(int id)
        {
            var result = await _userService.DeactivateUserAsync(id);
            return result switch
            {
                ActionResultEnum.ActionResult.InvalidData => BadRequest("Invalid data"),
                ActionResultEnum.ActionResult.NotFound => NotFound("User not found"),
                ActionResultEnum.ActionResult.AlreadyInactive => BadRequest("User is already inactive"),
                ActionResultEnum.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivateUserAsync(int id)
        {
            var result = await _userService.ActivateUserAsync(id);
            return result switch
            {
                ActionResultEnum.ActionResult.InvalidData => BadRequest("Invalid data"),
                ActionResultEnum.ActionResult.NotFound => NotFound("User not found"),
                ActionResultEnum.ActionResult.AlreadyActive => BadRequest("User is already active"),
                ActionResultEnum.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }

        //Ownership Policy
        [HttpPatch("{id}/password")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePasswordAsync(int id, [FromBody] ChangePasswordRequest req, [FromServices] IAuthorizationService authorizationService)
        {
            var authResult = await authorizationService.AuthorizeAsync(User, id, "UserOwnerOrSuperOrAdmin");

            if (!authResult.Succeeded)
                return Forbid();

            var result = await _userService.UpdatePassword(id, req.NewPassword, req.CurrentPassword);

            return result switch
            {
                ActionResultEnum.ActionResult.InvalidData => BadRequest("Invalid data"),
                ActionResultEnum.ActionResult.NotFound => NotFound("User not found"),
                ActionResultEnum.ActionResult.InvalidPassword => BadRequest("Invalid current password"),
                ActionResultEnum.ActionResult.WeakPassword => BadRequest("New password does not meet strength requirements"),
                ActionResultEnum.ActionResult.Success => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}