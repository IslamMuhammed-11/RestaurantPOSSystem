using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;

namespace Contracts.DTOs.ErrorsDTOs
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;

        public ErrorCodes.enErrorCodes Details { get; set; }
    }
}