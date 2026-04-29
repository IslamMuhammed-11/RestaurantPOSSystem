using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API_Layer.Authorization.User
{
    public class UserOwnerOrSuperOrAdminHandler : AuthorizationHandler<UserOwnerOrSuperOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context
            , UserOwnerOrSuperOrAdminRequirement requirement,
            int userId)
        {
            var user = context.User;

            if (user.IsInRole("Admin") || user.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(claim, out int currentUserId) && currentUserId == userId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}