using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.UserDTOs
{
    public class UserResponse
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}