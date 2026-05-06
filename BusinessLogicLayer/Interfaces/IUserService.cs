using Contracts.DTOs.UserDTOs;
using Contracts.Enums;
using Contracts.Result;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponse>> AddNewUserAsync(CreateUserRequest user);

        Task<Result<bool>> UpdateUsernameAsync(int ID, UpdateUserRequest user);

        Task<Result<UserResponse>> GetUserByIDAsync(int id);

        Task<Result<UserTokenData>> GetUserByUsernameAsync(string username);

        Task<Result<List<UserResponse>>> GetAllUsersAsync();

        Task<Result<bool>> DeleteUserByIDAsync(int id);

        Task<Result<bool>> DeactivateUserAsync(int UserID);

        Task<Result<bool>> ActivateUserAsync(int UserID);

        Task<Result<bool>> UpdatePassword(int UserID, string NewPassword, string Password);

        Task<Result<bool>> IsUserValid(int UserID);

        Task<Result<bool>> SaveRefreshTokenAsync(UserTokenData user);

        Task<Result<bool>> RevokeToken(int userId);
    }
}