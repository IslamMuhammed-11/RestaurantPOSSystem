using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entites
{
    public class UserEntity
    {

        public int UserID { get; set; }
        
        public required int  PersonID { get; set; }
        public required int RoleID { get; set; }
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
        public required bool IsActive { get; set; } = true;
    }
}
