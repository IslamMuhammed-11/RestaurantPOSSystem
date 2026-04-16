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

        public async Task<int?> AddNewUserAsync(CreateUserDTO user)
        {
            //Validating the user data
            if (!user.IsValid())
                return null;

            var PersonEntity = PersonMap.ToEntity(user.createPersonDTO);

            int? PersonID = await _personRepo.AddNewPersonAsync(PersonEntity);

            if (PersonID is null)
                return null;
            else
                user.PersonID = PersonID.Value;

            //Mapping the DTO to an Entity
            var userEntity = UserMap.ToEntity(user);

            return await _userRepo.AddNewUserAsync(userEntity);
        }

        public async Task<List<ReadUserDTO>> GetAllUsersAsync()
        {
            var userEntities = await _userRepo.GetAllUserAsync();

            return UserMap.ToReadDTOList(userEntities);
        }

        public async Task<ReadUserDTO?> GetUserByIDAsync(int ID)
        {
            var userEntity = await _userRepo.GetUserByIDAsync(ID);

            if (userEntity is null)
                return null;

            return UserMap.ToReadDTO(userEntity);
        }

        public async Task<Enums.ActionResult> UpdateUserAsync(int id, UpdateUserDTO user)
        {
            if (id <= 0)
                return Enums.ActionResult.InvalidData;

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Enums.ActionResult.NotFound;

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
            //Should be hashed and salted in a real application, this is just for demonstration purposes
            if (existingUser.PasswordHash != Passoword)
                return Enums.ActionResult.InvalidPassword;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return Enums.ActionResult.WeakPassword;

            //Should be hashed and salted in a real application, this is just for demonstration purposes
            if (!await _userRepo.UpdatePasswordAsync(id, newPassword))
                return Enums.ActionResult.DBError;

            return Enums.ActionResult.Success;
        }

        public async Task<bool> IsUserValid(int UserId)
        {
            return await _userRepo.DoesUserExistAsync(UserId);
        }
    }
}