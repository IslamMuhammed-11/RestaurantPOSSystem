using Contracts.DTOs.CustomerDTOs;
using Contracts.DTOs.RolesDTOs;
using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IRolesService
    {
        Task<int?> AddNewRoleAsync(CreateRoleRequest role);

        Task<ActionResultEnum.ActionResult> UpdateRoleAsync(int ID, UpdateRoleRequest role);

        Task<RoleResponse?> GetRoleByIDAsync(int id);

        Task<List<RoleResponse>> GetAllRolesAsync();

        Task<ActionResultEnum.ActionResult> DeleteRoleByIDAsync(int id);
    }
}