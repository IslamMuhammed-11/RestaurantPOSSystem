namespace Contracts.DTOs.OrderItemsDTOs
{
    public class AddItemDTO
    {
        public int ProductID { get; set; }
        public short Quantity { get; set; }

        //In the Future We Will Make A Product Available Check Before Adding To The Order From The Product Service
        //public bool isProductAvailable(IOrderItemService)
        public bool isValid() => ProductID > 0 && Quantity > 0;
    }
}