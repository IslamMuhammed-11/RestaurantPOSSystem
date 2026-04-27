using System;
using System.Collections.Generic;

namespace Contracts.DTOs.OrderDTOs
{
    public class OrderResponse
    {
        public int OrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? TableID { get; set; }
        public string OrderStatusName { get; set; } = string.Empty;
        public string OrderTypeName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
