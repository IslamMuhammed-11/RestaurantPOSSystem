using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ErrorHandling
{
    public class Error
    {
        public string Message { get; }
        public Contracts.Enums.ErrorCodes.enErrorCodes Code { get; }

        public Error(string message, Contracts.Enums.ErrorCodes.enErrorCodes code)
        {
            Message = message;
            Code = code;
        }
    }
}