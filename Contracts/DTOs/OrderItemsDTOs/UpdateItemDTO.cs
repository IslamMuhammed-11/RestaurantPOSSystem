namespace Contracts.DTOs.OrderItemsDTOs
{
    public class UpdateItemDTO
    {
        public int ItemID { get; set; }
        public int? ProductID { get; set; }
        public int? OrderID { get; set; }
        public short? Quantity { get; set; }
        public decimal? Price { get; set; }

        public bool IsValid() => ItemID > 0;
    }
}
