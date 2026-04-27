using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.CategoryDTOs;
using Contracts.DTOs.OrderDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepo _itemRepo;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public ItemService(IItemRepo itemRepo, IProductService productService, IOrderService orderService)
        {
            _itemRepo = itemRepo;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<int?> AddNewItemAsync(CreateOrderItemRequest item, int orderId)
        {
            if (item == null || !item.IsValid())
                throw new BusinessException("Invalid item data.", 80000, Enums.ActionResult.InvalidData);

            if (!await _productService.IsProductAvailableAsync(item.ProductID))
                throw new BusinessException("Product Isn't Available Or Doesn't Exist", 80002, Enums.ActionResult.InvalidData);

            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new BusinessException("Order Not Found!", 80004, Enums.ActionResult.NotFound);

            if (!_CheckOrderStatus(order))
                throw new BusinessException("Can't make changes to the order because it's cancelled , ready or completed ", 80006, Enums.ActionResult.Conflict);

            var entity = ItemMap.ToEntity(item, orderId);

            int? id = await _itemRepo.AddNewItemAsync(entity);
            if (!id.HasValue)
                throw new BusinessException("Failed to add item.", 80001, Enums.ActionResult.DBError);

            return id;
        }

        public async Task<OrderItemResponse?> GetItemByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("Invalid item ID.", 80000, Enums.ActionResult.InvalidData);

            var entity = await _itemRepo.GetItemByIdAsync(id);
            if (entity == null)
                return null;

            return ItemMap.ToReadDTO(entity);
        }

        public async Task<bool> UpdateItemsAsync(UpdateOrderItemRequest item)
        {
            if (item == null || !item.IsValid())
                throw new BusinessException("Invalid item data.", 80000, Enums.ActionResult.InvalidData);

            var existing = await _itemRepo.GetItemByIdAsync(item.ItemID);
            if (existing == null)
                throw new BusinessException("Item not found.", 80001, Enums.ActionResult.NotFound);

            if (item.ProductID.HasValue)
            {
                if (!await _productService.IsProductAvailableAsync(item.ProductID.Value))
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

        public async Task<bool> UpdateQuantityAsync(UpdateOrderItemQuantityRequest quantity, int orderID, int ItemId)
        {
            if (quantity.Quantity <= 0)
                throw new BusinessException($"The sent quantity isn't valid {quantity.Quantity}", 80001, Enums.ActionResult.InvalidData);

            var orderAndItems = await _orderService.GetOrderAndItemsByIdAsync(orderID);

            if (orderAndItems.Order == null)
                throw new BusinessException($"Order With This ID = {orderID} Was Not Found", 8050, Enums.ActionResult.NotFound);

            if (!_CheckOrderStatus(orderAndItems.Order))
                throw new BusinessException("Can't make changes to the order because it's cancelled , ready or completed ", 80006, Enums.ActionResult.Conflict);

            bool itemExist = orderAndItems.Items != null && orderAndItems.Items.Any(s => s.ItemID == ItemId);
            if (!itemExist)
                throw new BusinessException($"Item With This ID = {ItemId} Was Not Found In This Order {orderID}", 8050, Enums.ActionResult.NotFound);

            var ItemEntity = new ItemsEntity
            {
                ItemID = ItemId,
                OrderID = orderID,
                Quantity = (short)quantity.Quantity,
                Price = 0,
                ProductID = 0
            };

            bool isUpdated = await _itemRepo.UpdateQuantityAsync(ItemEntity);

            if (!isUpdated)
                throw new BusinessException("an unexpected error occured", 99999, Enums.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DeleteItemsAsync(int orderId, int ItemId)
        {
            if (ItemId <= 0)
                throw new BusinessException("Invalid item ID.", 80000, Enums.ActionResult.InvalidData);

            var orderAndItems = await _orderService.GetOrderAndItemsByIdAsync(orderId);

            if (orderAndItems.Order == null)
                throw new BusinessException($"Order with this ID {orderId} was not found", 80052, Enums.ActionResult.NotFound);

            if (!_CheckOrderStatus(orderAndItems.Order))
                throw new BusinessException("Can't make changes to the order because it's cancelled , ready or completed ", 80006, Enums.ActionResult.Conflict);

            bool itemExist = orderAndItems.Items != null && orderAndItems.Items.Select(s => s.ItemID == ItemId).Any();
            if (!itemExist)
                throw new BusinessException("Item not found.", 80001, Enums.ActionResult.NotFound);

            bool deleted = await _itemRepo.DeleteItemsAsync(ItemId);
            if (!deleted)
                throw new BusinessException("Failed to delete item.", 80002, Enums.ActionResult.DBError);

            return true;
        }

        private bool _CheckOrderStatus(OrderResponse order)
        {
            return order.OrderStatusName.ToLower() switch
            {
                "cancelled" => false,
                "ready" => false,
                "completed" => false,
                _ => true
            };
        }
    }
}