namespace Contracts.DTOs.OrderItemsDTOs
{
    public class CreateOrderItemRequest
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public bool IsValid() => ProductID > 0 && Quantity > 0;
    }
}
