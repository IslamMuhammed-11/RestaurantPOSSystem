using Contracts.DTOs.UserDTOs;
using Contracts.Enums;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<int?> AddNewUserAsync(CreateUserRequest user);

        Task<Enums.ActionResult> UpdateUsernameAsync(int ID, UpdateUserRequest user);

        Task<UserResponse?> GetUserByIDAsync(int id);

        Task<UserTokenData?> GetUserByUsernameAsync(string username);

        Task<List<UserResponse>> GetAllUsersAsync();

        Task<Enums.ActionResult> DeleteUserByIDAsync(int id);

        Task<Enums.ActionResult> DeactivateUserAsync(int UserID);

        Task<Enums.ActionResult> ActivateUserAsync(int UserID);

        Task<Enums.ActionResult> UpdatePassword(int UserID, string NewPassword, string Password);

        Task<bool> IsUserValid(int UserID);

        Task<bool> SaveRefreshTokenAsync(UserTokenData user);

        Task<bool> RevokeToken(int userId);
    }
}