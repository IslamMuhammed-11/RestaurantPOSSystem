using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserRepo
    {
        Task<UserEntity?> GetUserByIDAsync(int id);

        Task<UserEntity?> GetUserByUsernameAsync(string username);

        Task<List<UserEntity>> GetAllUserAsync();

        Task<int?> AddNewUserAsync(UserEntity person);

        Task<bool> UpdateUserAsync(UserEntity person);

        Task<bool> DeleteUserAsync(int UserID);

        Task<bool> DeactivateUserAsync(int UserID);

        Task<bool> ActivateUserAsync(int UserID);

        Task<bool> UpdatePasswordAsync(int UserID, string NewPassword);

        Task<bool> DoesUserExistAsync(int UserID);

        Task<bool> SaveRefreshTokenAsync(UserEntity person);

        Task<bool> RevokeToken(int id);
    }
}