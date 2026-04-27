using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.UserDTOs;
using Contracts.Enums;
using DataAccessLayer.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IPersonRepo _personRepo;

        public UserService(IUserRepo userRepo, IPersonRepo Person)
        {
            _userRepo = userRepo;
            _personRepo = Person;
        }

        public async Task<int?> AddNewUserAsync(CreateUserRequest user)
        {
            //Validating the user data
            if (!user.IsValid())
                return null;

            var PersonEntity = PersonMap.ToEntity(user.PersonData);

            int? PersonID = await _personRepo.AddNewPersonAsync(PersonEntity);

            if (PersonID is null)
                return null;
            else
                user.PersonID = PersonID.Value;

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            //Mapping the DTO to an Entity
            var userEntity = UserMap.ToEntity(user);

            return await _userRepo.AddNewUserAsync(userEntity);
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            var userEntities = await _userRepo.GetAllUserAsync();

            return UserMap.ToReadDTOList(userEntities);
        }

        public async Task<UserResponse?> GetUserByIDAsync(int ID)
        {
            var userEntity = await _userRepo.GetUserByIDAsync(ID);

            if (userEntity is null)
                return null;

            return UserMap.ToReadDTO(userEntity);
        }

        public async Task<UserTokenData?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var userEntity = await _userRepo.GetUserByUsernameAsync(username);

            if (userEntity is null)
                return null;

            return UserMap.ToReadDTOWithPasswordHash(userEntity);
        }

        public async Task<Enums.ActionResult> UpdateUserAsync(int id, UpdateUserRequest user)
        {
            if (id <= 0)
                return Enums.ActionResult.InvalidData;

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Enums.ActionResult.NotFound;

            //Checking if the Password is Changed
            if (!string.IsNullOrEmpty(user.Password) && !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.PasswordHash))
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if (!UserMap.ToEntity(user, existingUser))
                return Enums.ActionResult.Error;

            if (!await _userRepo.UpdateUserAsync(existingUser))
                return Enums.ActionResult.DBError;

            return Enums.ActionResult.Success;
        }

        public async Task<Enums.ActionResult> DeleteUserByIDAsync(int id)
        {
            if (id <= 0)
                return Enums.ActionResult.InvalidData;

            if (!await _userRepo.DoesUserExistAsync(id))
                return Enums.ActionResult.NotFound;

            if (!await _userRepo.DeleteUserAsync(id))
                return Enums.ActionResult.DBError;

            return Enums.ActionResult.Success;
        }

        public async Task<Enums.ActionResult> DeactivateUserAsync(int id)
        {
            if (id <= 0)
                return Enums.ActionResult.InvalidData;

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Enums.ActionResult.NotFound;

            if (!existingUser.IsActive)
                return Enums.ActionResult.AlreadyInactive;

            if (!await _userRepo.DeactivateUserAsync(id))
                return Enums.ActionResult.DBError;

            return Enums.ActionResult.Success;
        }

        public async Task<Enums.ActionResult> ActivateUserAsync(int id)
        {
            if (id <= 0)
                return Enums.ActionResult.InvalidData;

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Enums.ActionResult.NotFound;

            if (existingUser.IsActive)
                return Enums.ActionResult.AlreadyActive;

            if (!await _userRepo.ActivateUserAsync(id))
                return Enums.ActionResult.DBError;

            return Enums.ActionResult.Success;
        }

        public async Task<Enums.ActionResult> UpdatePassword(int id, string newPassword, string Passoword)
        {
            if (id <= 0)
                return Enums.ActionResult.InvalidData;

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Enums.ActionResult.NotFound;

            if (!existingUser.IsActive)
                return Enums.ActionResult.InActiveUser;

            if (!BCrypt.Net.BCrypt.Verify(Passoword, existingUser.PasswordHash))
                return Enums.ActionResult.InvalidPassword;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return Enums.ActionResult.WeakPassword;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (!await _userRepo.UpdatePasswordAsync(id, hashedPassword))
                return Enums.ActionResult.DBError;

            return Enums.ActionResult.Success;
        }

        public async Task<bool> IsUserValid(int UserId)
        {
            return await _userRepo.DoesUserExistAsync(UserId);
        }

        public async Task<bool> SaveRefreshTokenAsync(UserTokenData user)
        {
            var entity = UserMap.ToEntity(user);

            return await _userRepo.SaveRefreshTokenAsync(entity);
        }

        public async Task<bool> RevokeToken(int userId)
        {
            return await _userRepo.RevokeToken(userId);
        }
    }
}