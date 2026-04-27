using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.OrderDTOs;
using Contracts.DTOs.OrderItemsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Layer.Controllers
{
    [Authorize]
    [Route("api/OrdersController")]
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

        [HttpPost("order/create")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder(Contracts.DTOs.OrderDTOs.CreateOrderRequest order)
        {
            if (order == null || !order.IsValid())
                return BadRequest("Null Was Sent");
            int? newOrderId;

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                    return Unauthorized("Invalid Token");

                int createdByUserId = int.Parse(userId);

                newOrderId = await _orderService.CreateOrderAsync(order, createdByUserId);
            }
            catch (Exception ex)
            {
                if (ex is BusinessException businessEx)
                {
                    return businessEx.ErrorType switch
                    {
                        Enums.ActionResult.InvalidData => BadRequest(businessEx.Message),
                        Enums.ActionResult.NotFound => NotFound(businessEx.Message),
                        Enums.ActionResult.Conflict => Conflict(businessEx.Message),
                        Enums.ActionResult.DBError => StatusCode(StatusCodes.Status500InternalServerError, businessEx.Message),
                        _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                    };
                }

                if (ex is ArgumentException argEx)
                    return BadRequest(argEx.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }

            if (!newOrderId.HasValue)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create order.");

            var ReadyOrder = await _orderService.GetOrderByIdAsync(newOrderId.Value);

            var Response = new OrderCreatedResponse
            {
                OrderID = ReadyOrder.OrderID,
                StatusName = ReadyOrder.OrderStatusName,
                OrderTypeName = ReadyOrder.OrderTypeName,
                TotalPrice = ReadyOrder.TotalPrice,
                CreatedAt = ReadyOrder.CreatedAt
            };
            return CreatedAtRoute("GetOrderById", new { id = newOrderId.Value }, Response);
        }

        //Ownership policy will be applied here for waiter
        [HttpGet("order/all")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (BusinessException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Ownership policy will be applied here for waiter
        [HttpGet("order/{id}", Name = "GetOrderById")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                // Incase of Exceptions It Throws Not Found And DB Error Business Exceptions
                var order = await _orderService.GetOrderAndItemsByIdAsync(id);

                return Ok(order);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
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
            try
            {
                bool isUpdated = await _orderService.ChangeOrderStatus(id, orderStatus);
                if (!isUpdated)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update order status.");
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("order/{id}/preparing")]
        [Authorize(Roles = "Admin,SuperAdmin,Kitchen")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsPreparing(int id)
        {
            return await ChangeStatus(id, IOrderService.enOrderStatus.Preparing);
        }

        [HttpPut("order/{id}/ready")]
        [Authorize(Roles = "Admin,SuperAdmin,Kitchen")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsReady(int id)
        {
            return await ChangeStatus(id, IOrderService.enOrderStatus.Ready);
        }

        [HttpPut("order/{id}/cancelled")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAsCancelled(int id)
        {
            return await ChangeStatus(id, IOrderService.enOrderStatus.Cancelled);
        }

        [HttpPut("order/{id}/ChangeTable/{TableID}")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeTable(int id, int TableID)
        {
            try
            {
                bool isUpdated = await _orderService.ChangeTable(id, TableID);
                if (!isUpdated)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to change table.");
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("order/{Id}/Item")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddNewItem(int Id, CreateOrderItemRequest item)
        {
            try
            {
                int? newItemID = await _itemService.AddNewItemAsync(item, Id);

                if (!newItemID.HasValue)
                    return StatusCode(500, "Unexpected error occurd");

                return Ok("Item was added successfully");
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.Conflict => StatusCode(409, ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpPut("order/{orderId}/Item{ItemId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeItemQuantity(int orderId, int ItemId, UpdateOrderItemQuantityRequest quantity)
        {
            try
            {
                bool isChanged = await _itemService.UpdateQuantityAsync(quantity, orderId, ItemId);

                if (!isChanged)
                    return StatusCode(500, "Unexpected error occured");

                var order = await _orderService.GetOrderByIdAsync(orderId);

                return Ok(order);
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.Conflict => StatusCode(409, ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }

        [HttpDelete("order/{orderId}/Item{itemId}")]
        [Authorize(Roles = "Admin,SuperAdmin,Cashier,Waiter")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteItem(int orderId, int itemId)
        {
            try
            {
                bool isDeleted = await _itemService.DeleteItemsAsync(orderId, itemId);

                if (!isDeleted)
                    return StatusCode(500, "Unexpected error occured");

                return NoContent();
            }
            catch (BusinessException ex)
            {
                return ex.ErrorType switch
                {
                    Enums.ActionResult.InvalidData => BadRequest(ex.Message),
                    Enums.ActionResult.NotFound => NotFound(ex.Message),
                    Enums.ActionResult.Conflict => StatusCode(409, ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }
    }
}