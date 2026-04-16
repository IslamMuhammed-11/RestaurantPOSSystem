using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.CustomerDTOs
{
    public class ReadCustomerDTO
    {
        public required int CustomerID { get; set; }
        public required string Name { get; set; }
        public string? Phone { get; set; }
    }
}
