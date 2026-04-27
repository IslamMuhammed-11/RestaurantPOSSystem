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

        public int PersonID { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public string? RefreshTokenHash { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}