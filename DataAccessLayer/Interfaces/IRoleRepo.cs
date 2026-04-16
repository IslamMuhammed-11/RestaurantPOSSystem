using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IRoleRepo
    {
        Task<RoleEntity?> GetRoleByIDAsync(int id);
        Task<List<RoleEntity>> GetAllRoleAsync();
        Task<int?> AddNewRoleAsync(RoleEntity role);
        Task<bool> UpdateRoleAsync(RoleEntity role);
        Task<bool> DeleteRoleAsync(int roleID);
    }
}
