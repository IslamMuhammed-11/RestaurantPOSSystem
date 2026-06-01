using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.UserDTOs;
using Contracts.Enums;
using DataAccessLayer.Interfaces;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IPersonRepo _personRepo;
        private readonly IRolesService _rolesService;

        public UserService(IUserRepo userRepo, IPersonRepo Person, IRolesService rolesService)
        {
            _userRepo = userRepo;
            _personRepo = Person;
            _rolesService = rolesService;
        }

        public async Task<Result<UserResponse>> AddNewUserAsync(CreateUserRequest user)
        {
            //Validating the user data
            if (!user.IsValid())
                return Result<UserResponse>.Failure(new Error("Invalid user data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var roleResult = await _rolesService.GetRoleByIDAsync(user.RoleID);

            if (!roleResult.IsSuccess || roleResult.Value is null)
                return Result<UserResponse>.Failure(new Error("Role not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            var PersonEntity = PersonMap.ToEntity(user.PersonData);

            int? PersonID = await _personRepo.AddNewPersonAsync(PersonEntity);

            if (PersonID is null)
                return Result<UserResponse>.Failure(new Error("Failed to create person.", ErrorCodes.enErrorCodes.DB_ERROR));
            else
                user.PersonID = PersonID.Value;

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            //Mapping the DTO to an Entity
            var userEntity = UserMap.ToEntity(user);

            int? userId = await _userRepo.AddNewUserAsync(userEntity);

            if (!userId.HasValue)
                return Result<UserResponse>.Failure(new Error("Failed to create user userRepo Returned null.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new UserResponse
            {
                UserID = userId.Value,
                RoleID = user.RoleID,
                Role = userEntity.Role,
                UserName = userEntity.UserName,
                IsActive = true
            };

            return Result<UserResponse>.Success(response);
        }

        public async Task<Result<List<UserResponse>>> GetAllUsersAsync()
        {
            var userEntities = await _userRepo.GetAllUserAsync();

            return Result<List<UserResponse>>.Success(UserMap.ToReadDTOList(userEntities));
        }

        public async Task<Result<UserResponse>> GetUserByIDAsync(int ID)
        {
            var userEntity = await _userRepo.GetUserByIDAsync(ID);

            if (userEntity is null)
                return Result<UserResponse>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<UserResponse>.Success(UserMap.ToReadDTO(userEntity));
        }

        public async Task<Result<UserTokenData>> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Result<UserTokenData>.Failure(new Error("Invalid username.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var userEntity = await _userRepo.GetUserByUsernameAsync(username);

            if (userEntity is null)
                return Result<UserTokenData>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<UserTokenData>.Success(UserMap.ToReadDTOWithPasswordHash(userEntity));
        }

        public async Task<Result<bool>> UpdateUsernameAsync(int id, UpdateUserRequest user)
        {
            if (id <= 0)
                return Result<bool>.Failure(new Error("Invalid user ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Result<bool>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            UserMap.ToEntity(user, existingUser);

            if (!await _userRepo.UpdateUserAsync(existingUser))
                return Result<bool>.Failure(new Error("Failed to update user.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteUserByIDAsync(int id)
        {
            if (id <= 0)
                return Result<bool>.Failure(new Error("Invalid user ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            if (!await _userRepo.DoesUserExistAsync(id))
                return Result<bool>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!await _userRepo.DeleteUserAsync(id))
                return Result<bool>.Failure(new Error("Failed to delete user.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeactivateUserAsync(int id)
        {
            if (id <= 0)
                return Result<bool>.Failure(new Error("Invalid user ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Result<bool>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!existingUser.IsActive)
                return Result<bool>.Failure(new Error("User is already inactive.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (!await _userRepo.DeactivateUserAsync(id))
                return Result<bool>.Failure(new Error("Failed to deactivate user.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ActivateUserAsync(int id)
        {
            if (id <= 0)
                return Result<bool>.Failure(new Error("Invalid user ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Result<bool>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (existingUser.IsActive)
                return Result<bool>.Failure(new Error("User is already active.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (!await _userRepo.ActivateUserAsync(id))
                return Result<bool>.Failure(new Error("Failed to activate user.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdatePassword(int id, string newPassword, string Passoword)
        {
            if (id <= 0)
                return Result<bool>.Failure(new Error("Invalid user ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existingUser = await _userRepo.GetUserByIDAsync(id);

            if (existingUser is null)
                return Result<bool>.Failure(new Error("User not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!existingUser.IsActive)
                return Result<bool>.Failure(new Error("User is inactive.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (!BCrypt.Net.BCrypt.Verify(Passoword, existingUser.PasswordHash))
                return Result<bool>.Failure(new Error("Invalid password.", ErrorCodes.enErrorCodes.INVALID_DATA));

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return Result<bool>.Failure(new Error("Weak password.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (!await _userRepo.UpdatePasswordAsync(id, hashedPassword))
                return Result<bool>.Failure(new Error("Failed to update password.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> IsUserValid(int UserId)
        {
            return Result<bool>.Success(await _userRepo.DoesUserExistAsync(UserId));
        }

        public async Task<Result<bool>> SaveRefreshTokenAsync(UserTokenData user)
        {
            var entity = UserMap.ToEntity(user);

            if (!await _userRepo.SaveRefreshTokenAsync(entity))
                return Result<bool>.Failure(new Error("Failed to save refresh token.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RevokeToken(int userId)
        {
            if (!await _userRepo.RevokeToken(userId))
                return Result<bool>.Failure(new Error("Failed to revoke token.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(true);
        }
    }
}