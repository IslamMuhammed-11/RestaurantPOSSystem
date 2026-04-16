using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.OrderDTOs
{
    public class UpdateOrderDTO
    {
        public int? TableID { get; set; }

        public int? OrderStatus { get; set; }

        public string? Notes { get; set; }

    }
}
