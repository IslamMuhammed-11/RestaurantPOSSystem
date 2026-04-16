namespace Contracts.DTOs.OrderDTOs
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public int? CustomerID { get; set; }
        public int? TableID { get; set; }
        public string OrderStatusName { get; set; }
        public string OrderTypeName { get; set; }
        public decimal TotalPrice { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }

        //Should Return The Order Items With The Product Details Like Name And Price To Avoid Multiple Calls To The Product Service When Displaying The Order Details
        //public List<OrderItemsDTOs.OrderItemDTO> Items { get; set; } = new List<OrderItemsDTOs.OrderItemDTO>();
    }
}