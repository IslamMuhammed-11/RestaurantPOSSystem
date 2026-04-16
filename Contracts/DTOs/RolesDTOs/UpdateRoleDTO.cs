using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.RolesDTOs
{
    public class UpdateRoleDTO
    {
        public string? RoleName { get; set; }
        public short? Premission { get; set; }
    }
}
