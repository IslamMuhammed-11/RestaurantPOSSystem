namespace Contracts.DTOs.OrderItemsDTOs
{
    public class OrderItemResponse
    {
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}