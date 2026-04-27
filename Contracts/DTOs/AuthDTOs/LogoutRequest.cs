using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.AuthDTOs
{
    public class LogoutRequest
    {
        public string Username { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}