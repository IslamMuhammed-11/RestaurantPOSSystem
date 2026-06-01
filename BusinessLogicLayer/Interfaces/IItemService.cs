using Contracts.DTOs.OrderItemsDTOs;
using System.Threading.Tasks;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface IItemService
    {
        Task<Result<OrderItemResponse>> GetItemByIdAsync(int id);

        Task<Result<CreateOrderItemResponse>> AddNewItemAsync(CreateOrderItemRequest item, int orderId);

        Task<Result<bool>> UpdateItemsAsync(UpdateOrderItemRequest item);

        Task<Result<bool>> UpdateQuantityAsync(UpdateOrderItemQuantityRequest quantity, int orderID, int ItemId);

        Task<Result<bool>> DeleteItemsAsync(int OrderId, int ItemId);
    }
}