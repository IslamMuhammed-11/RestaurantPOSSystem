using Microsoft.AspNetCore.Authorization;

namespace API_Layer.Authorization.User
{
    public class UserOwnerOrSuperOrAdminRequirement : IAuthorizationRequirement
    {
    }
}