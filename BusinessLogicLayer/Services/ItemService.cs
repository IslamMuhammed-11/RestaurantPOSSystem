using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.CategoryDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepo _itemRepo;
        private readonly IProductService _productService;
        private readonly IOrderRepo _orderRepo;
        public ItemService(IItemRepo itemRepo , IProductService productService , IOrderRepo orderRepo)
        {
            _itemRepo = itemRepo;
            _productService = productService;
            _orderRepo = orderRepo;
        }    

        public async Task<int?> AddNewItemAsync(AddItemDTO item, int orderId)
        {
            if (item == null || !item.isValid())
                throw new BusinessException("Invalid item data.", 80000, Enums.ActionResult.InvalidData);

            if (!await _productService.IsProductAvailableAsync(item.ProductID))
                throw new BusinessException("Product Isn't Available Or Doesn't Exist", 80002, Enums.ActionResult.InvalidData);

            var order = await _orderRepo.GetOrderByIDAsync(orderId);

            if (order == null)
                throw new BusinessException("Order Not Found!", 80004, Enums.ActionResult.NotFound);

            if((int)order.OrderStatus >= (int)OrderEntity.enOrderStatus.Ready)
                throw new BusinessException("Can't make changes to the order because it's cancelled , ready or completed " , 80006 , Enums.ActionResult.Conflict);



            var entity = ItemMap.ToEntity(item, orderId);

            int? id = await _itemRepo.AddNewItemAsync(entity);
            if (!id.HasValue)
                throw new BusinessException("Failed to add item.", 80001, Enums.ActionResult.DBError);

            return id;
        }

        public async Task<ReadItemDTO?> GetItemByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("Invalid item ID.", 80000, Enums.ActionResult.InvalidData);

            var entity = await _itemRepo.GetItemByIdAsync(id);
            if (entity == null)
                return null;

            return ItemMap.ToReadDTO(entity);
        }

        public async Task<bool> UpdateItemsAsync(UpdateItemDTO item)
        {
            if (item == null || !item.IsValid())
                throw new BusinessException("Invalid item data.", 80000, Enums.ActionResult.InvalidData);

            var existing = await _itemRepo.GetItemByIdAsync(item.ItemID);
            if (existing == null)
                throw new BusinessException("Item not found.", 80001, Enums.ActionResult.NotFound);

            if (item.ProductID.HasValue)
            {  
                if (await _productService.IsProductAvailableAsync(item.ProductID.Value))
                    throw new BusinessException("Product Isn't Available Or Doesn't Exist", 80002, Enums.ActionResult.InvalidData);
            }

            bool ok = ItemMap.ToEntity(item, existing);
            if (!ok)
                throw new BusinessException("Invalid item data.", 80000, Enums.ActionResult.InvalidData);

            bool updated = await _itemRepo.UpdateItemsAsync(existing);
            if (!updated)
                throw new BusinessException("Failed to update item.", 80002, Enums.ActionResult.DBError);

            return true;
        }

        public async Task<bool> UpdateQuantityAsync(UpdateQuantityDTO quantity , int orderID , int ItemId)
        {


            if (quantity.Qunatity <= 0)
                throw new BusinessException($"The sent quantity isn't valid {quantity.Qunatity}" , 80001 , Enums.ActionResult.InvalidData);

            var order = await _orderRepo.GetOrderByIDAsync(orderID);
            if(order == null)
                throw new BusinessException($"Order With This ID = {orderID} Was Not Found" , 8050 ,  Enums.ActionResult.NotFound);

            if((int)order.OrderStatus >= (int)OrderEntity.enOrderStatus.Ready)
                throw new BusinessException("Can't make changes to the order because it's cancelled , ready or completed ", 80006, Enums.ActionResult.Conflict);

            var Item = await _itemRepo.GetItemByIdAsync(ItemId);

            if(Item == null)
                throw new BusinessException($"Item With This ID = {ItemId} Was Not Found" , 8050 , Enums.ActionResult.NotFound);

            if (Item.OrderID != order.OrderID)
                throw new BusinessException($"Item Was Not compatiable with the sent order with ID = {orderID} ", 8055, Enums.ActionResult.Conflict);

            var ItemEntity = new ItemsEntity
            {
                ItemID = ItemId,
                OrderID = orderID,
                Quantity = quantity.Qunatity,
                Price = 0,
                ProductID = 0
            };

            bool isUpdated = await _itemRepo.UpdateQuantityAsync(ItemEntity);

            if (!isUpdated)
                throw new BusinessException("an unexpected error occured", 99999, Enums.ActionResult.DBError);

            return true;

        }

        public async Task<bool> DeleteItemsAsync(int orderId,int ItemId)
        {
            if (ItemId <= 0)
                throw new BusinessException("Invalid item ID.", 80000, Enums.ActionResult.InvalidData);

            var order = await _orderRepo.GetOrderByIDAsync(orderId);

            if (order == null)
                throw new BusinessException($"Order with this ID {orderId} was not found", 80052, Enums.ActionResult.NotFound);

            if((int)order.OrderStatus >= (int)OrderEntity.enOrderStatus.Ready)
                throw new BusinessException("Can't make changes to the order because it's cancelled , ready or completed ", 80006, Enums.ActionResult.Conflict);

            var existing = await _itemRepo.GetItemByIdAsync(ItemId);
            if (existing == null)
                throw new BusinessException("Item not found.", 80001, Enums.ActionResult.NotFound);

            bool deleted = await _itemRepo.DeleteItemsAsync(ItemId);
            if (!deleted)
                throw new BusinessException("Failed to delete item.", 80002, Enums.ActionResult.DBError);

            return true;
        }
    }
}
