using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.RolesDTOs;
using Contracts.Enums;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class RolesService : IRolesService
    {
        private readonly IRoleRepo _roleRepo;

        public RolesService(IRoleRepo roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<int?> AddNewRoleAsync(CreateRoleRequest role)
        {
            if (role == null || !role.IsValid())
                return null;

            var roleEntity = RolesMap.ToEntity(role);

            int? ID = await _roleRepo.AddNewRoleAsync(roleEntity);

            return ID;
        }

        public async Task<RoleResponse?> GetRoleByIDAsync(int id)
        {
            if (id < 0)
                return null;
            var roleEntity = await _roleRepo.GetRoleByIDAsync(id);
            if (roleEntity == null)
                return null;
            return RolesMap.ToReadDTO(roleEntity);
        }

        public async Task<Enums.ActionResult> UpdateRoleAsync(int id, UpdateRoleRequest role)
        {
            if (role == null || id < 0)
                return Enums.ActionResult.InvalidData;

            var existingRole = await _roleRepo.GetRoleByIDAsync(id);
            if (existingRole == null)
                return Enums.ActionResult.NotFound;

            Mapping.RolesMap.ToEntity(role, existingRole);

            if (!await _roleRepo.UpdateRoleAsync(existingRole))
                return Enums.ActionResult.Error;

            return Enums.ActionResult.Success;
        }

        public async Task<List<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await _roleRepo.GetAllRoleAsync();
            return roles.Select(RolesMap.ToReadDTO).ToList();
        }

        public async Task<Enums.ActionResult> DeleteRoleByIDAsync(int id)
        {
            if (id < 0)
                return Enums.ActionResult.InvalidData;

            var existingRole = await _roleRepo.GetRoleByIDAsync(id);
            if (existingRole == null)
                return Enums.ActionResult.NotFound;

            if (!await _roleRepo.DeleteRoleAsync(id))
                return Enums.ActionResult.Error;

            return Enums.ActionResult.Success;
        }
    }
}