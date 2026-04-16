using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.RolesDTOs
{
    public class CreateRoleDTO
    {
        private int _ID;

        public int RoleID { get { return _ID; } }
        public required string RoleName { get; set; }
        public required short Premission { get; set; }
        public void SetID(int id) => _ID = id;
        public bool isValid() => !string.IsNullOrEmpty(RoleName);
    }
}
