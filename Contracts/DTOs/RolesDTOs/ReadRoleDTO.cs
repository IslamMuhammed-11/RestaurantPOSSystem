using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.RolesDTOs
{
    public class ReadRoleDTO
    {
        public required int RoleID { get; set; }
        public required string RoleName { get; set; }
        public required short Premission { get; set; }
    }
}
