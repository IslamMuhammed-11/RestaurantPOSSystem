using Contracts.DTOs.UserDTOs;
using Contracts.Enums;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<int?> AddNewUserAsync(CreateUserRequest user);

        Task<ActionResultEnum.ActionResult> UpdateUsernameAsync(int ID, UpdateUserRequest user);

        Task<UserResponse?> GetUserByIDAsync(int id);

        Task<UserTokenData?> GetUserByUsernameAsync(string username);

        Task<List<UserResponse>> GetAllUsersAsync();

        Task<ActionResultEnum.ActionResult> DeleteUserByIDAsync(int id);

        Task<ActionResultEnum.ActionResult> DeactivateUserAsync(int UserID);

        Task<ActionResultEnum.ActionResult> ActivateUserAsync(int UserID);

        Task<ActionResultEnum.ActionResult> UpdatePassword(int UserID, string NewPassword, string Password);

        Task<bool> IsUserValid(int UserID);

        Task<bool> SaveRefreshTokenAsync(UserTokenData user);

        Task<bool> RevokeToken(int userId);
    }
}