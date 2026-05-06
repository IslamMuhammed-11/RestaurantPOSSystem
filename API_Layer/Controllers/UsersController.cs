using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.UserDTOs;
using Contracts.Enums;
using Microsoft.AspNetCore.Authorization;
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
            var result = await _userService.GetUserByIDAsync(id);

            return ResultMappingExtensions.ToActionResult(result);
        }

        [HttpGet()]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var result = await _userService.GetAllUsersAsync();

            return ResultMappingExtensions.ToActionResult(result);
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
            var result = await _userService.AddNewUserAsync(user);

            if (!result.IsSuccess)
                return ResultMappingExtensions.ToActionResult(result);

            return CreatedAtRoute("GetUserByID", new { id = result.Value.UserID }, result.Value);
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
                return StatusCode(403, new ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = "You don’t have permission to perform this action",
                    Status = 403
                });

            var result = await _userService.UpdateUsernameAsync(id, user);

            return ResultMappingExtensions.ToActionResult(result, $"Username updated successfully to {user.Username}");
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

            return ResultMappingExtensions.ToActionResult(result, null, false);
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

            return ResultMappingExtensions.ToActionResult(result, "User has been deactivated", false);
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

            return ResultMappingExtensions.ToActionResult(result, "User has been activated", false);
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
                return StatusCode(403, new ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = "You don’t have permission to perform this action",
                    Status = 403
                });

            var result = await _userService.UpdatePassword(id, req.NewPassword, req.CurrentPassword);

            return ResultMappingExtensions.ToActionResult(result, "Password has been updated", false);
        }
    }
}