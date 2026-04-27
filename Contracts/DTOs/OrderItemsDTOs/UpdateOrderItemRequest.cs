namespace Contracts.DTOs.OrderItemsDTOs
{
    public class UpdateOrderItemRequest
    {
        public int ItemID { get; set; }
        public int? ProductID { get; set; }
        public int? OrderID { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }

        public bool IsValid() => ItemID > 0;
    }
}
