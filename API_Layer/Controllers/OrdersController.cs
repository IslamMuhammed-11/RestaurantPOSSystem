using API_Layer.Mapping;
using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.OrderDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Contracts.DTOs.BaseResponse;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IItemService _itemService;

        public OrdersController(IOrderService orderService, IItemService itemService)
        {
            _orderService = orderService;
            _itemService = itemService;
        }

        [HttpPost()]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateOrder(Contracts.DTOs.OrderDTOs.CreateOrderRequest order)
        {
            if (order == null || !order.IsValid())
                return ResultMappingExtensions.ToActionResult(Result<CreateOrderRequest>.Failure(new Error("Invalid order data", ErrorCodes.enErrorCodes.INVALID_DATA)));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return ResultMappingExtensions.ToActionResult(Result<CreateOrderRequest>.Failure(new Error("User ID not found in token", ErrorCodes.enErrorCodes.AUTHENCATION_FAILED)));

            int createdByUserId = int.Parse(userId);

            var creationResult = await _orderService.CreateOrderAsync(order, createdByUserId);

            if (!creationResult.IsSuccess)
                return Mapping.ResultMappingExtensions.ToActionResult(creationResult);

            return CreatedAtRoute("GetOrderById", new { id = creationResult.Value.Order.OrderID }, ApiResponse<OrderWithItemsResponse>.Success(creationResult.Value));
        }

        //Ownership policy will be applied here for waiter
        //Kitchen should see active orders too
        //Probably Will be Solved By Filtering.
        [HttpGet()]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Kitchen")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Mapping.ResultMappingExtensions.ToActionResult(orders);
        }

        //Ownership policy will be applied here for waiter
        //Probably Waiter will be addded in the roles
        [HttpGet("{id}", Name = "GetOrderById")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Kitchen")]
        [EnableRateLimiting("UserLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            // Incase of Exceptions It Throws Not Found And DB Error Business Exceptions
            var order = await _orderService.GetOrderAndItemsByIdAsync(id);

            return Mapping.ResultMappingExtensions.ToActionResult(order);
        }

        //[HttpPut]
        //public async Task<IActionResult> UpdateOrder(int id, Contracts.DTOs.OrderDTOs.UpdateOrderDTO order)
        //{
        //    if (order == null || !order.IsValid())
        //        return BadRequest("Null Was Sent");
        //    try
        //    {
        //        bool isUpdated = await _orderService.UpdateOrderAsync(id, order);
        //        if (!isUpdated)
        //            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update order.");
        //        return NoContent();
        //    }
        //    catch (BusinessException ex)
        //    {
        //        return ex.ErrorType switch
        //        {
        //            Enums.ActionResult.InvalidData => BadRequest(ex.Message),
        //            Enums.ActionResult.NotFound => NotFound(ex.Message),
        //            _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
        //        };
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        return BadRequest(argEx.Message);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        //    }
        //}

        private async Task<IActionResult> ChangeStatus(int id, IOrderService.enOrderStatus orderStatus)
        {
            var result = await _orderService.ChangeOrderStatus(id, orderStatus);

            return ResultMappingExtensions.ToActionResult(result, $"Order marked as {orderStatus.ToString()}");
        }

        [HttpPatch("{id}/preparing")]
        [Authorize(Roles = "Admin,SuperAdmin,Kitchen")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsPreparing(int id)
        {
            return await ChangeStatus(id, IOrderService.enOrderStatus.Preparing);
        }

        [HttpPatch("{id}/ready")]
        [Authorize(Roles = "Admin,SuperAdmin,Kitchen")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsReady(int id)
        {
            return await ChangeStatus(id, IOrderService.enOrderStatus.Ready);
        }

        [HttpPatch("{id}/cancelled")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MarkAsCancelled(int id)
        {
            return await ChangeStatus(id, IOrderService.enOrderStatus.Cancelled);
        }

        [HttpPatch("{id}/table")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeTable(int id, [FromBody] ChangeTableRequest req)
        {
            var result = await _orderService.ChangeTable(id, req);

            return Mapping.ResultMappingExtensions.ToActionResult(result, "Changed successfully");
        }

        [HttpPost("{Id}/items")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewItem(int Id, CreateOrderItemRequest item)
        {
            var result = await _itemService.AddNewItemAsync(item, Id);

            return Mapping.ResultMappingExtensions.ToActionResult(result, "Item was added successfully");
        }

        [HttpPatch("{orderId}/items/{ItemId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeItemQuantity(int orderId, int ItemId, UpdateOrderItemQuantityRequest quantity)
        {
            var result = await _itemService.UpdateQuantityAsync(quantity, orderId, ItemId);

            if (!result.IsSuccess)
                return Mapping.ResultMappingExtensions.ToActionResult(result);

            var order = await _orderService.GetOrderByIdAsync(orderId);

            return ResultMappingExtensions.ToActionResult(order, "Quantity changed successfully");
        }

        [HttpDelete("{orderId}/items/{itemId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [EnableRateLimiting("OrderLimiter")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteItem(int orderId, int itemId)
        {
            var isDeleted = await _itemService.DeleteItemsAsync(orderId, itemId);

            return ResultMappingExtensions.ToActionResult(isDeleted, null, false);
        }
    }
}