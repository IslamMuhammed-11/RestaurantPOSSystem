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
        Task<int?> AddNewRoleAsync(CreateRoleDTO role);
        Task<Enums.ActionResult> UpdateRoleAsync(int ID, UpdateRoleDTO role);
        Task<ReadRoleDTO?> GetRoleByIDAsync(int id);
        Task<List<ReadRoleDTO>> GetAllRolesAsync();
        Task<Enums.ActionResult> DeleteRoleByIDAsync(int id);
    }
}
