using Contracts.DTOs.OrderDTOs;
using Contracts.Enums;
using Contracts.Result;

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

        Task<Result<OrderWithItemsResponse>> CreateOrderAsync(CreateOrderRequest order, int createdByUserId);

        Task<Result<List<OrderResponse>>> GetAllOrdersAsync();

        Task<Result<OrderWithItemsResponse>> GetOrderAndItemsByIdAsync(int orderId);

        Task<Result<OrderResponse>> GetOrderByIdAsync(int orderId);

        Task<bool> UpdateOrderAsync(int orderId, UpdateOrderRequest order);

        Task<Result<bool>> ChangeOrderStatus(int Id, enOrderStatus OrderStatus);

        Task<Result<bool>> ChangeTable(int Id, ChangeTableRequest req);
    }
}