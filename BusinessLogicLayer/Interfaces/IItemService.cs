using Contracts.DTOs.OrderItemsDTOs;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IItemService
    {
        Task<OrderItemResponse?> GetItemByIdAsync(int id);

        Task<int?> AddNewItemAsync(CreateOrderItemRequest item, int orderId);

        Task<bool> UpdateItemsAsync(UpdateOrderItemRequest item);

        Task<bool> UpdateQuantityAsync(UpdateOrderItemQuantityRequest quantity , int orderID , int ItemId);

        Task<bool> DeleteItemsAsync(int OrderId , int ItemId);
    }
}
