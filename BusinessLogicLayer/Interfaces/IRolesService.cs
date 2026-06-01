using Contracts.DTOs.CustomerDTOs;
using Contracts.DTOs.RolesDTOs;
using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface IRolesService
    {
        Task<Result<int>> AddNewRoleAsync(CreateRoleRequest role);

        Task<Result<bool>> UpdateRoleAsync(int ID, UpdateRoleRequest role);

        Task<Result<RoleResponse>> GetRoleByIDAsync(int id);

        Task<Result<List<RoleResponse>>> GetAllRolesAsync();

        Task<Result<bool>> DeleteRoleByIDAsync(int id);
    }
}