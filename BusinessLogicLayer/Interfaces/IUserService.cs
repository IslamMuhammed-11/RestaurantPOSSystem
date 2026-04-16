using Contracts.DTOs.UserDTOs;
using Contracts.Enums;

namespace BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<int?> AddNewUserAsync(CreateUserDTO user);

        Task<Enums.ActionResult> UpdateUserAsync(int ID, UpdateUserDTO user);

        Task<ReadUserDTO?> GetUserByIDAsync(int id);

        Task<List<ReadUserDTO>> GetAllUsersAsync();

        Task<Enums.ActionResult> DeleteUserByIDAsync(int id);

        Task<Enums.ActionResult> DeactivateUserAsync(int UserID);

        Task<Enums.ActionResult> ActivateUserAsync(int UserID);

        Task<Enums.ActionResult> UpdatePassword(int UserID, string NewPassword, string Password);

        Task<bool> IsUserValid(int UserID);
    }
}