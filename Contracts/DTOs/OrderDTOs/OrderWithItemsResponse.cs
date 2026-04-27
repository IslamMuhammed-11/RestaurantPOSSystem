using Contracts.DTOs.OrderItemsDTOs;
using System.Collections.Generic;

namespace Contracts.DTOs.OrderDTOs
{
    public class OrderWithItemsResponse
    {
        public OrderResponse Order { get; set; } = new OrderResponse();
        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }
}
