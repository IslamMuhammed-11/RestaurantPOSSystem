using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.OrderDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;

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

        public async Task<int?> CreateOrderAsync(CreateOrderRequest order, int createByUserId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Ensure Order Data Are Not Bad Numbers
            if (!order.IsValid())
                throw new ArgumentException("Invalid order data.");

            //Check for duplicate ProductIDs in the order items
            bool isDuplicated = (order.Items.Count != order.Items.Select(x => x.ProductID).Distinct().Count());

            // Ensure all items are valid and there are no duplicate ProductIDs
            if (order.Items.All(item => !item.IsValid())
                || isDuplicated)
                throw new ArgumentException("Duplicates or Invalid Items");

            //In The Future I Will Send the Invalid Data to the Client To Tell Him Which Products Are Invalid Or Duplicated Instead Of Just Sending A Message
            var InvalidProducts = await _productService.ValidateProducts(order.Items.Select(x => x.ProductID).ToList());

            if (InvalidProducts.Any())
                throw new BusinessException("Some Products Are Not Found Or Unavaliable", 50020, Enums.ActionResult.InvalidData);

            // Ensure OrderType is valid and TableID is provided for DineIn orders
            if (order.OrderType < (int)OrderEntity.enOrderType.DineIn || order.OrderType > (int)OrderEntity.enOrderType.Delivery)
                throw new ArgumentException("Invalid Order Type");

            if (order.OrderType == (int)OrderEntity.enOrderType.DineIn && !order.TableID.HasValue)
                throw new BusinessException("Table is required for DineIn orders.", 50009, Enums.ActionResult.InvalidData);

            if (order.TableID.HasValue && order.OrderType != (int)OrderEntity.enOrderType.DineIn)
                throw new BusinessException("TableID should be null for non-DineIn orders.", 50011, Enums.ActionResult.InvalidData);

            bool IsUserValid = await _userService.IsUserValid(createByUserId);
            // Ensure UserID is valid and active
            if (!IsUserValid)
                throw new BusinessException("Invalid UserID Does not Exist Or Inactive", 50010, Enums.ActionResult.InvalidData);

            // Map CreateOrderDTO to OrderEntity
            var NewOrder = Mapping.OrderMap.ToOrderEntity(order);

            NewOrder.CreatedByUserID = createByUserId;

            // Convert List<CreateOrderItemRequest> to DataTable for bulk insert
            var orderItemsTable = OrderMap.ConvertToDataTable<CreateOrderItemRequest>(order.Items);

            // Call repository to create order and return the new OrderID
            return await _orderRepo.CreateOrderAsync(NewOrder, orderItemsTable);
        }

        public async Task<OrderWithItemsResponse> GetOrderAndItemsByIdAsync(int orderId)
        {
            // Incase of Error it Throws BusinessException With DBError And SqlException message
            var orderAndItemsEntity = await _orderRepo.GetOrderAndItemsByOrderIDAsync(orderId);

            if (orderAndItemsEntity == null)
                throw new BusinessException($"Order With This ID Was not found {orderId}", 90005, Enums.ActionResult.NotFound);

            return Mapping.OrderMap.ToOrderAndItemsDTO(orderAndItemsEntity);
        }

        public async Task<OrderResponse> GetOrderByIdAsync(int id)
        {
            var orderEntity = await _orderRepo.GetOrderByIDAsync(id);

            if (orderEntity == null)
                throw new BusinessException($"Order with this Id was not found {id}", 90005, Enums.ActionResult.NotFound);

            return OrderMap.ToOrderResponse(orderEntity);
        }

        public async Task<List<OrderResponse>> GetAllOrdersAsync()
        {
            var orderEntities = await _orderRepo.GetAllOrdersAsync();
            return orderEntities.Select(oe => Mapping.OrderMap.ToOrderResponse(oe)).ToList();
        }

        public async Task<bool> UpdateOrderAsync(int orderId, UpdateOrderRequest order)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ChangeOrderStatus(int Id, IOrderService.enOrderStatus orderStatus)
        {
            var orderEntity = await _orderRepo.GetOrderByIDAsync(Id);

            if (orderEntity == null)
                throw new BusinessException("Order Not Found", 50012, Enums.ActionResult.NotFound);

            //Declaring the newStatus variable here to use it in both the validation and the repository call, ensuring consistency and avoiding redundant code.
            var newStatus = (OrderEntity.enOrderStatus)orderStatus;
            // Check if the order is already in the specified status to avoid unnecessary updates.
            if (orderEntity.OrderStatus == newStatus)
                throw new BusinessException("Order is already in the specified status.", 50013, Enums.ActionResult.InvalidData);

            // Checking if the order is cancelled, as cancelled orders should not have their status changed.
            if (orderEntity.OrderStatus == OrderEntity.enOrderStatus.Cancelled)
                throw new BusinessException("Cannot change status of a cancelled order.", 50014, Enums.ActionResult.InvalidData);

            // Validating the status transition rules to ensure that the order moves through the correct stages (Pending -> Preparing -> Ready).
            if (newStatus == OrderEntity.enOrderStatus.Ready && orderEntity.OrderStatus != OrderEntity.enOrderStatus.Preparing)
                throw new BusinessException("Order must be in Preparing status before it can be marked as Ready.", 50015, Enums.ActionResult.InvalidData);

            if (newStatus == OrderEntity.enOrderStatus.Preparing && orderEntity.OrderStatus != OrderEntity.enOrderStatus.Pending)
                throw new BusinessException("Order must be in Pending status before it can be marked as Preparing.", 50016, Enums.ActionResult.InvalidData);

            if (newStatus == OrderEntity.enOrderStatus.Cancelled && (orderEntity.OrderStatus == OrderEntity.enOrderStatus.Ready || orderEntity.OrderStatus == OrderEntity.enOrderStatus.Completed))
                throw new BusinessException("Cannot cancel an order that is already Ready or Completed.", 50017, Enums.ActionResult.InvalidData);

            return await _orderRepo.ChangeOrderStatus(Id, newStatus);
        }

        public async Task<bool> ChangeTable(int Id, int TableID)
        {
            var orderEntity = await _orderRepo.GetOrderByIDAsync(Id);
            if (orderEntity == null)
                throw new BusinessException("Order Not Found", 50012, Enums.ActionResult.NotFound);

            if (orderEntity.OrderType != OrderEntity.enOrderType.DineIn)
                throw new BusinessException("Only DineIn orders can have their table changed.", 50018, Enums.ActionResult.InvalidData);

            if (orderEntity.TableID == TableID)
                throw new BusinessException("Cannot Change To The Same Table", 50019, Enums.ActionResult.InvalidData);

            if (orderEntity.OrderStatus == OrderEntity.enOrderStatus.Cancelled || orderEntity.OrderStatus == OrderEntity.enOrderStatus.Completed)
                throw new BusinessException("Order is cancelled or completed", 50020, Enums.ActionResult.Conflict);

            return await _orderRepo.ChangeTable(Id, TableID);
        }
    }
}