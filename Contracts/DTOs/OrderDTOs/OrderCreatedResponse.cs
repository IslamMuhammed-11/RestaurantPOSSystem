using System;

namespace Contracts.DTOs.OrderDTOs
{
    public class OrderCreatedResponse
    {
        public int OrderID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string OrderTypeName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
