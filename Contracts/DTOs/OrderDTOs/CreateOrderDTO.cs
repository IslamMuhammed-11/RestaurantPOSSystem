namespace Contracts.DTOs.OrderDTOs
{
    public class CreateOrderDTO
    {
        private int _OrderID;
        public int OrderID
        { get { return _OrderID; } }
        public int? CustomerID { get; set; }
        public int? TableID { get; set; }
        public string? Notes { get; set; }
        public int CreatedByUserID { get; set; }
        public int OrderType { get; set; }
        public List<OrderItemsDTOs.AddItemDTO> Items { get; set; } = new List<OrderItemsDTOs.AddItemDTO>();

        public void SetOrderID(int id) => _OrderID = id;

        public bool IsValid()
        {
            if (CreatedByUserID <= 0 || OrderType <= 0 || Items.Count == 0)
                return false;



            return true; 
        }
    }
}