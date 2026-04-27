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
        public static RoleEntity ToEntity(CreateRoleRequest role)
        {
            return new RoleEntity
            {
                RoleName = role.Name,
                Premission = 0
            };
        }

        public static void ToEntity(UpdateRoleRequest role, RoleEntity existingRole)
        {
            if (role.Name != null)
                existingRole.RoleName = role.Name;
        }

        public static RoleResponse ToReadDTO(RoleEntity role)
        {
            return new RoleResponse
            {
                RoleID = role.RoleID,
                Name = role.RoleName
            };
        }
    }
}