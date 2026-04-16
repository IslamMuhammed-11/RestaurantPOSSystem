using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.OrderDTOs
{
    public class CreateOrderResponseDTO
    {
        public int OrderID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string OrderTypeName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
