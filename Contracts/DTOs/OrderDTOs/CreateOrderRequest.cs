using Contracts.DTOs.OrderItemsDTOs;
using System.Collections.Generic;

namespace Contracts.DTOs.OrderDTOs
{
    public class CreateOrderRequest
    {
        private int _OrderID;
        public int OrderID
        { get { return _OrderID; } }
        public int? CustomerID { get; set; }
        public int? TableID { get; set; }
        public string? Notes { get; set; }
        public int OrderType { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new List<CreateOrderItemRequest>();

        public void SetOrderID(int id) => _OrderID = id;

        public bool IsValid()
        {
            if (OrderType <= 0 || Items.Count == 0)
                return false;
            return true;
        }
    }
}