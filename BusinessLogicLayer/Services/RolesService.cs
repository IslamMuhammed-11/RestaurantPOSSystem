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
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Services
{
    public class RolesService : IRolesService
    {
        private readonly IRoleRepo _roleRepo;

        public RolesService(IRoleRepo roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<Result<int>> AddNewRoleAsync(CreateRoleRequest role)
        {
            if (role == null || !role.IsValid())
                return Result<int>.Failure(new Error("Invalid role data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var roleEntity = RolesMap.ToEntity(role);

            int? ID = await _roleRepo.AddNewRoleAsync(roleEntity);

            if (!ID.HasValue)
                return Result<int>.Failure(new Error("Failed to add new role.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<int>.Success(ID.Value);
        }

        public async Task<Result<RoleResponse>> GetRoleByIDAsync(int id)
        {
            if (id < 0)
                return Result<RoleResponse>.Failure(new Error("Invalid role ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var roleEntity = await _roleRepo.GetRoleByIDAsync(id);
            if (roleEntity == null)
                return Result<RoleResponse>.Failure(new Error("Role not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<RoleResponse>.Success(RolesMap.ToReadDTO(roleEntity));
        }

        public async Task<Result<bool>> UpdateRoleAsync(int id, UpdateRoleRequest role)
        {
            if (role == null || id < 0)
                return Result<bool>.Failure(new Error("Invalid role data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existingRole = await _roleRepo.GetRoleByIDAsync(id);
            if (existingRole == null)
                return Result<bool>.Failure(new Error("Role not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            Mapping.RolesMap.ToEntity(role, existingRole);

            bool updated = await _roleRepo.UpdateRoleAsync(existingRole);

            return Result<bool>.Success(updated);
        }

        public async Task<Result<List<RoleResponse>>> GetAllRolesAsync()
        {
            var roles = await _roleRepo.GetAllRoleAsync();

            return Result<List<RoleResponse>>.Success(roles.Select(RolesMap.ToReadDTO).ToList());
        }

        public async Task<Result<bool>> DeleteRoleByIDAsync(int id)
        {
            if (id < 0)
                return Result<bool>.Failure(new Error("Invalid role ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existingRole = await _roleRepo.GetRoleByIDAsync(id);
            if (existingRole == null)
                return Result<bool>.Failure(new Error("Role not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool deleted = await _roleRepo.DeleteRoleAsync(id);

            if (!deleted)
                return Result<bool>.Failure(new Error("Failed to delete role.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(deleted);
        }
    }
}