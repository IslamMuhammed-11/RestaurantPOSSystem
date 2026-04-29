using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.UserDTOs;
using DataAccessLayer.Entites;

namespace BusinessLogicLayer.Mapping
{
    public class UserMap
    {
        public static UserResponse ToReadDTO(UserEntity entity)
        {
            return new UserResponse
            {
                UserID = entity.UserID,
                RoleID = entity.RoleID,
                Role = entity.Role,
                UserName = entity.UserName,
                IsActive = entity.IsActive
            };
        }

        public static UserTokenData ToReadDTOWithPasswordHash(UserEntity entity)
        {
            return new UserTokenData
            {
                UserID = entity.UserID,
                RoleID = entity.RoleID,
                Role = entity.Role,
                UserName = entity.UserName,
                IsActive = entity.IsActive,
                PasswordHash = entity.PasswordHash,
                RefreshTokenHash = entity.RefreshTokenHash,
                ExpiresAt = entity.ExpiresAt,
                RevokedAt = entity.RevokedAt
            };
        }

        public static UserEntity ToEntity(CreateUserRequest dto)
        {
            return new UserEntity
            {
                PersonID = dto.PersonID,
                RoleID = dto.RoleID,
                UserName = dto.UserName,
                PasswordHash = dto.Password,//Was Hashed Before it gets here
                IsActive = dto.IsActive
            };
        }

        public static bool ToEntity(UpdateUserRequest dto, UserEntity user)
        {
            if (dto == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Username))
                user.UserName = dto.Username;

            return true;
        }

        public static UserEntity ToEntity(UserTokenData user)
        {
            return new UserEntity
            {
                UserID = user.UserID,
                RoleID = user.RoleID,
                Role = user.Role,
                UserName = user.UserName,
                IsActive = user.IsActive,
                PasswordHash = user.PasswordHash,
                RefreshTokenHash = user.RefreshTokenHash,
                ExpiresAt = user.ExpiresAt,
                RevokedAt = user.RevokedAt
            };
        }

        public static List<UserResponse> ToReadDTOList(List<UserEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<UserResponse>();

            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    }
}