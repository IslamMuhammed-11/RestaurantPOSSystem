namespace Contracts.DTOs.OrderItemsDTOs
{
    public class UpdateOrderItemQuantityRequest
    {
        public int Quantity { get; set; }
        public bool IsValid() => Quantity > 0;
    }
}
