namespace Contracts.DTOs.OrderDTOs
{
    public class UpdateOrderRequest
    {
        public int? TableID { get; set; }

        public int? OrderStatus { get; set; }

        public string? Notes { get; set; }

    }
}
