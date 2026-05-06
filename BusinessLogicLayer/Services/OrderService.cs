using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.OrderDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Contracts.Result;

namespace BusinessLogicLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public OrderService(IOrderRepo orderRepo, IUserService userService, IProductService productService)
        {
            _orderRepo = orderRepo;
            _userService = userService;
            _productService = productService;
        }

        public OrderEntity.enOrderStatus enStatus;

        public async Task<Result<OrderWithItemsResponse>> CreateOrderAsync(CreateOrderRequest order, int createByUserId)
        {
            if (order == null)
                return Result<OrderWithItemsResponse>.Failure(new Error("Order cannot be null.", ErrorCodes.enErrorCodes.INVALID_DATA));

            // Ensure Order Data Are Not Bad Numbers
            if (!order.IsValid())
                return Result<OrderWithItemsResponse>.Failure(new Error("Invalid order data.", ErrorCodes.enErrorCodes.INVALID_DATA));
            //Check for duplicate ProductIDs in the order items
            bool isDuplicated = (order.Items.Count != order.Items.Select(x => x.ProductID).Distinct().Count());

            // Ensure all items are valid and there are no duplicate ProductIDs
            if (order.Items.All(item => !item.IsValid())
                || isDuplicated)
                return Result<OrderWithItemsResponse>.Failure(new Error("Duplicates or Invalid Items", ErrorCodes.enErrorCodes.INVALID_DATA));

            //In The Future I Will Send the Invalid Data to the Client To Tell Him Which Products Are Invalid Or Duplicated Instead Of Just Sending A Message
            var InvalidProducts = await _productService.ValidateProducts(order.Items.Select(x => x.ProductID).ToList());

            if (InvalidProducts.Any())
                return Result<OrderWithItemsResponse>.Failure(new Error("Products with Ids " + string.Join(", ", InvalidProducts) + " are invalid or unavailable.", ErrorCodes.enErrorCodes.INVALID_DATA));

            // Ensure OrderType is valid and TableID is provided for DineIn orders
            if (order.OrderType < (int)OrderEntity.enOrderType.DineIn || order.OrderType > (int)OrderEntity.enOrderType.Delivery)
                return Result<OrderWithItemsResponse>.Failure(new Error("Invalid Order Type", ErrorCodes.enErrorCodes.INVALID_DATA));
            if (order.OrderType == (int)OrderEntity.enOrderType.DineIn && !order.TableID.HasValue)
                return Result<OrderWithItemsResponse>.Failure(new Error("Table is required for DineIn orders.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (order.TableID.HasValue && order.OrderType != (int)OrderEntity.enOrderType.DineIn)
                return Result<OrderWithItemsResponse>.Failure(new Error("TableID should be null for non-DineIn orders.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            var user = await _userService.GetUserByIDAsync(createByUserId);
            // Ensure UserID is valid and active
            if (!user.IsSuccess || !user.Value.IsActive)
                return Result<OrderWithItemsResponse>.Failure(new Error("Invalid UserID Does not Exist Or Inactive", ErrorCodes.enErrorCodes.INVALID_DATA));

            // Map CreateOrderDTO to OrderEntity
            var NewOrder = Mapping.OrderMap.ToOrderEntity(order);

            NewOrder.CreatedByUserID = createByUserId;

            // Convert List<CreateOrderItemRequest> to DataTable for bulk insert
            var orderItemsTable = OrderMap.ConvertToDataTable<CreateOrderItemRequest>(order.Items);

            // Call repository to create order and return the new OrderID
            var newOrderId = await _orderRepo.CreateOrderAsync(NewOrder, orderItemsTable);

            if (!newOrderId.HasValue)
                return Result<OrderWithItemsResponse>.Failure(new Error("Failed to create order.", ErrorCodes.enErrorCodes.DB_ERROR));

            var orderRes = await _orderRepo.GetOrderAndItemsByOrderIDAsync(newOrderId.Value);

            var response = new OrderWithItemsResponse
            {
                Order = Mapping.OrderMap.ToOrderResponse(orderRes!.Order),
                Items = orderRes.Items.Select(oi => Mapping.ItemMap.ToReadDTO(oi)).ToList()
            };

            return Result<OrderWithItemsResponse>.Success(response);
        }

        public async Task<Result<OrderWithItemsResponse>> GetOrderAndItemsByIdAsync(int orderId)
        {
            // Incase of Error it Throws BusinessException With DBError And SqlException message
            var orderAndItemsEntity = await _orderRepo.GetOrderAndItemsByOrderIDAsync(orderId);

            if (orderAndItemsEntity == null)
                return Result<OrderWithItemsResponse>.Failure(new Error($"Order with this Id was not found {orderId}", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<OrderWithItemsResponse>.Success(Mapping.OrderMap.ToOrderAndItemsDTO(orderAndItemsEntity));
        }

        public async Task<Result<OrderResponse>> GetOrderByIdAsync(int id)
        {
            var orderEntity = await _orderRepo.GetOrderByIDAsync(id);

            if (orderEntity == null)
                return Result<OrderResponse>.Failure(new Error($"Order with this Id was not found {id}", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<OrderResponse>.Success(OrderMap.ToOrderResponse(orderEntity));
        }

        public async Task<Result<List<OrderResponse>>> GetAllOrdersAsync()
        {
            var orderEntities = await _orderRepo.GetAllOrdersAsync();

            var orderResponses = orderEntities.Select(oe => Mapping.OrderMap.ToOrderResponse(oe)).ToList();

            return Result<List<OrderResponse>>.Success(orderResponses);
        }

        public async Task<bool> UpdateOrderAsync(int orderId, UpdateOrderRequest order)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> ChangeOrderStatus(int Id, IOrderService.enOrderStatus orderStatus)
        {
            var orderEntity = await _orderRepo.GetOrderByIDAsync(Id);

            if (orderEntity == null)
                return Result<bool>.Failure(new Error("Order Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));

            //Declaring the newStatus variable here to use it in both the validation and the repository call, ensuring consistency and avoiding redundant code.
            var newStatus = (OrderEntity.enOrderStatus)orderStatus;
            // Check if the order is already in the specified status to avoid unnecessary updates.
            if (orderEntity.OrderStatus == newStatus)
                return Result<bool>.Failure(new Error("Order is already in the specified status.", ErrorCodes.enErrorCodes.INVALID_DATA));

            // Checking if the order is cancelled, as cancelled orders should not have their status changed.
            if (orderEntity.OrderStatus == OrderEntity.enOrderStatus.Cancelled)
                return Result<bool>.Failure(new Error("Cannot change status of a cancelled order.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));
            // Validating the status transition rules to ensure that the order moves through the correct stages (Pending -> Preparing -> Ready).
            if (newStatus == OrderEntity.enOrderStatus.Ready && orderEntity.OrderStatus != OrderEntity.enOrderStatus.Preparing)
                return Result<bool>.Failure(new Error("Order must be in Preparing status before it can be marked as Ready.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (newStatus == OrderEntity.enOrderStatus.Preparing && orderEntity.OrderStatus != OrderEntity.enOrderStatus.Pending)
                return Result<bool>.Failure(new Error("Order must be in Pending status before it can be marked as Preparing.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (newStatus == OrderEntity.enOrderStatus.Cancelled && (orderEntity.OrderStatus == OrderEntity.enOrderStatus.Ready || orderEntity.OrderStatus == OrderEntity.enOrderStatus.Completed))
                return Result<bool>.Failure(new Error("Cannot cancel an order that is already Ready or Completed.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            var result = await _orderRepo.ChangeOrderStatus(Id, newStatus);

            if (!result)
                return Result<bool>.Failure(new Error("Failed to change order status.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(result);
        }

        public async Task<Result<bool>> ChangeTable(int Id, ChangeTableRequest req)
        {
            var orderEntity = await _orderRepo.GetOrderByIDAsync(Id);
            if (orderEntity == null)
                return Result<bool>.Failure(new Error("Order Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));
            if (orderEntity.OrderType != OrderEntity.enOrderType.DineIn)
                return Result<bool>.Failure(new Error("Only DineIn orders can have their table changed.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            if (orderEntity.TableID == req.TableID)
                return Result<bool>.Failure(new Error("Cannot Change To The Same Table", ErrorCodes.enErrorCodes.INVALID_DATA));

            if (orderEntity.OrderStatus == OrderEntity.enOrderStatus.Cancelled || orderEntity.OrderStatus == OrderEntity.enOrderStatus.Completed)
                return Result<bool>.Failure(new Error("Order is cancelled or completed", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            var result = await _orderRepo.ChangeTable(Id, req.TableID);

            if (!result)
                return Result<bool>.Failure(new Error("Failed to change table.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(result);
        }
    }
}