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
        public static ReadUserDTO ToReadDTO(UserEntity entity)
        {

            return new ReadUserDTO
            {
                UserID = entity.UserID,
                RoleID = entity.RoleID,
                UserName = entity.UserName,
                IsActive = entity.IsActive
            };
        }

        public static UserEntity ToEntity(CreateUserDTO dto)
        {

            return new UserEntity
            {
                PersonID = dto.PersonID,
                RoleID = dto.RoleID,
                UserName = dto.UserName,
                PasswordHash = dto.Password,// In a real application, you should hash the password before storing it
                IsActive = dto.IsActive
            };
        }

        public static bool ToEntity(UpdateUserDTO dto, UserEntity user)
        {
            if (dto == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Username))
                user.UserName = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = dto.Password; // In a real application, you should hash the password before storing it

            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;

            if (dto.RoleID.HasValue)
                user.RoleID = dto.RoleID.Value;

            return true;
        }

        public static List<ReadUserDTO> ToReadDTOList(List<UserEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<ReadUserDTO>();

            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    } 
}
