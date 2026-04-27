using Contracts.DTOs.RolesDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.UserDTOs
{
    public class UserTokenData
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshTokenHash { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
    }
}