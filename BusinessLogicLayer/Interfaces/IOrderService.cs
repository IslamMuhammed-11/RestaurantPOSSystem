using Contracts.DTOs.OrderDTOs;
using Contracts.Enums;

namespace BusinessLogicLayer.Interfaces
{
    public interface IOrderService
    {
        public enum enOrderStatus
        {
            Pending = 1,
            Preparing = 2,
            Ready = 3,
            Completed = 4,
            Cancelled = 5
        }

        Task<int?> CreateOrderAsync(CreateOrderRequest order, int createdByUserId);

        Task<List<OrderResponse>> GetAllOrdersAsync();

        Task<OrderWithItemsResponse> GetOrderAndItemsByIdAsync(int orderId);

        Task<OrderResponse> GetOrderByIdAsync(int orderId);

        Task<bool> UpdateOrderAsync(int orderId, UpdateOrderRequest order);

        Task<bool> ChangeOrderStatus(int Id, enOrderStatus OrderStatus);

        Task<bool> ChangeTable(int Id, int TableID);
    }
}