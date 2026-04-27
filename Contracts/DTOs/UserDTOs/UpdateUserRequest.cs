using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.UserDTOs
{
    public class UpdateUserRequest
    {
        public int? RoleID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
    }
}