using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.OrderDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.ErrorHandling;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;

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

        public async Task<Result<CreateOrderItemResponse>> AddNewItemAsync(CreateOrderItemRequest item, int orderId)
        {
            if (item == null || !item.IsValid())
                return Result<CreateOrderItemResponse>.Failure(new Error("Invalid item data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var product = await _productService.GetProductByIDAsync(item.ProductID);

            if (!product.IsSuccess || !product.Value.IsAvailable)
                return Result<CreateOrderItemResponse>.Failure(new Error("Product Isn't Available Or Doesn't Exist", ErrorCodes.enErrorCodes.INVALID_DATA));

            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (!order.IsSuccess)
                return Result<CreateOrderItemResponse>.Failure(new Error("Order Not Found!", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!_CheckOrderStatus(order.Value))
                return Result<CreateOrderItemResponse>.Failure(new Error("Can't make changes to the order because it's cancelled , ready or completed ", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            var entity = ItemMap.ToEntity(item, orderId);

            int? id = await _itemRepo.AddNewItemAsync(entity);

            if (!id.HasValue)
                return Result<CreateOrderItemResponse>.Failure(new Error("Failed to add new item.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new CreateOrderItemResponse
            {
                OrderItemID = id.Value,
                ProductID = item.ProductID,
                ProductName = product.Value.Name,
                Quantity = item.Quantity
            };

            return Result<CreateOrderItemResponse>.Success(response);
        }

        public async Task<Result<OrderItemResponse>> GetItemByIdAsync(int id)
        {
            if (id <= 0)
                return Result<OrderItemResponse>.Failure(new Error("Invalid item ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var entity = await _itemRepo.GetItemByIdAsync(id);
            if (entity == null)
                return Result<OrderItemResponse>.Failure(new Error("Item not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<OrderItemResponse>.Success(ItemMap.ToReadDTO(entity));
        }

        public async Task<Result<bool>> UpdateItemsAsync(UpdateOrderItemRequest item)
        {
            if (item == null || !item.IsValid())
                return Result<bool>.Failure(new Error("Invalid item data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _itemRepo.GetItemByIdAsync(item.ItemID);
            if (existing == null)
                return Result<bool>.Failure(new Error("Item not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (item.ProductID.HasValue)
            {
                var isProductAvailable = await _productService.IsProductAvailableAsync(item.ProductID.Value);

                if (!isProductAvailable.IsSuccess || !isProductAvailable.Value)
                    return Result<bool>.Failure(new Error("Product Isn't Available Or Doesn't Exist", ErrorCodes.enErrorCodes.INVALID_DATA));
            }

            bool ok = ItemMap.ToEntity(item, existing);
            if (!ok)
                return Result<bool>.Failure(new Error("Invalid item data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            bool updated = await _itemRepo.UpdateItemsAsync(existing);

            if (!updated)
                return Result<bool>.Failure(new Error("Failed to update item.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(updated);
        }

        public async Task<Result<bool>> UpdateQuantityAsync(UpdateOrderItemQuantityRequest quantity, int orderID, int ItemId)
        {
            if (quantity.Quantity <= 0)
                return Result<bool>.Failure(new Error($"The sent quantity isn't valid {quantity.Quantity}", ErrorCodes.enErrorCodes.INVALID_DATA));

            var orderAndItems = await _orderService.GetOrderAndItemsByIdAsync(orderID);

            if (!orderAndItems.IsSuccess || orderAndItems.Value.Order == null)
                return Result<bool>.Failure(new Error($"Order With This ID = {orderID} Was Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!_CheckOrderStatus(orderAndItems.Value.Order))
                return Result<bool>.Failure(new Error("Can't make changes to the order because it's cancelled , ready or completed ", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            bool itemExist = orderAndItems.Value.Items != null && orderAndItems.Value.Items.Any(s => s.ItemID == ItemId);
            if (!itemExist)
                return Result<bool>.Failure(new Error($"Item With This ID = {ItemId} Was Not Found In This Order {orderID}", ErrorCodes.enErrorCodes.NOT_FOUND));

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
                return Result<bool>.Failure(new Error("Failed to update item quantity.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(isUpdated);
        }

        public async Task<Result<bool>> DeleteItemsAsync(int orderId, int ItemId)
        {
            if (ItemId <= 0)
                return Result<bool>.Failure(new Error("Invalid item ID.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var orderAndItems = await _orderService.GetOrderAndItemsByIdAsync(orderId);

            if (orderAndItems.Value.Order == null)
                return Result<bool>.Failure(new Error($"Order with this ID {orderId} was not found", ErrorCodes.enErrorCodes.NOT_FOUND));

            if (!_CheckOrderStatus(orderAndItems.Value.Order))
                return Result<bool>.Failure(new Error("Can't make changes to the order because it's cancelled , ready or completed ", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            bool itemExist = orderAndItems.Value.Items != null && orderAndItems.Value.Items.Select(s => s.ItemID == ItemId).Any();
            if (!itemExist)
                return Result<bool>.Failure(new Error("Item not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool deleted = await _itemRepo.DeleteItemsAsync(ItemId);

            if (!deleted)
                return Result<bool>.Failure(new Error("Failed to delete item.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(deleted);
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