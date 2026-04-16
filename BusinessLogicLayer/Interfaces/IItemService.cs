using Contracts.DTOs.OrderItemsDTOs;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IItemService
    {
        Task<ReadItemDTO?> GetItemByIdAsync(int id);

        Task<int?> AddNewItemAsync(AddItemDTO item, int orderId);

        Task<bool> UpdateItemsAsync(UpdateItemDTO item);

        Task<bool> UpdateQuantityAsync(UpdateQuantityDTO quantity , int orderID , int ItemId);

        Task<bool> DeleteItemsAsync(int OrderId , int ItemId);
    }
}
