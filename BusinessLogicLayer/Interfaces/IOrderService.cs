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
        
        Task<int?> CreateOrderAsync(CreateOrderDTO order);

        Task<List<OrderDTO>> GetAllOrdersAsync();

        Task<OrderDTO> GetOrderByIdAsync(int orderId);

        Task<bool> UpdateOrderAsync(int orderId, UpdateOrderDTO order);

        Task<bool> ChangeOrderStatus(int Id , enOrderStatus OrderStatus);

        Task<bool> ChangeTable(int Id, int TableID);
    }
}