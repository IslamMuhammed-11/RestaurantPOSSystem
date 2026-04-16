namespace DataAccessLayer.Entites
{
    public class OrderEntity
    {
        public enum enOrderStatus
        {
            Pending = 1,
            Preparing = 2,
            Ready = 3,
            Completed = 4,
            Cancelled = 5
        }

        public enum enOrderType
        {
            DineIn = 1,
            TakeAway = 2,
            Delivery = 3
        }

        public int OrderID { get; set; }
        public int? CustomerID { get; set; }
        public int CreatedByUserID { get; set; }
        public string Username { get; set; }
        public int? TableID { get; set; }
        public enOrderStatus OrderStatus { get; set; }
        public string StatusName { get; set; }
        public enOrderType OrderType { get; set; }
        public string OrderTypeName { get; set; }
        public decimal TotalPrice { get; set; }
        public string? notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}