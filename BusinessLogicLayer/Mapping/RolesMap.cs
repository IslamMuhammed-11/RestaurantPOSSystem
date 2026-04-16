using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.RolesDTOs;
using Contracts.Enums;
namespace BusinessLogicLayer.Mapping
{
    public class RolesMap
    {

        public static RoleEntity ToEntity(CreateRoleDTO role)
        {
            return new RoleEntity
            {
                RoleName = role.RoleName,
                Premission = role.Premission
            };
        }

        public static void ToEntity(UpdateRoleDTO role , RoleEntity existingRole)
        {
            if (role.RoleName != null)
                existingRole.RoleName = role.RoleName;
            if(role.Premission != null)
                existingRole.Premission = (short)role.Premission;
        }

        public static ReadRoleDTO ToReadDTO(RoleEntity role)
        {
            return new ReadRoleDTO
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                Premission = role.Premission
            };
        }
    }
}
