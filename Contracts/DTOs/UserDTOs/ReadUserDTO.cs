using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.UserDTOs
{
    public class ReadUserDTO
    {
        public required int UserID { get; set; }
        public required int RoleID { get; set; }
        public required string UserName { get; set; }
        public required bool IsActive { get; set; }
    }
}
