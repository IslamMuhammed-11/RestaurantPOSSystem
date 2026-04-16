using Contracts.DTOs.OrderDTOs;
using DataAccessLayer.Entites;
using System.Data;

namespace BusinessLogicLayer.Mapping
{
    public class OrderMap
    {
        public static OrderDTO ToOrderDTO(OrderEntity order)
        {
            return new OrderDTO
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                TableID = order.TableID,
                OrderStatusName = order.StatusName,
                OrderTypeName = order.OrderTypeName,
                TotalPrice = order.TotalPrice,
                CreatedByUsername = order.Username,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Notes = order.notes
            };
        }

        public static List<OrderDTO> ToOrderDTOList(List<OrderEntity> orders)
        {
            return orders.Select(ToOrderDTO).ToList();
        }

        public static OrderEntity ToOrderEntity(CreateOrderDTO order)
        {
            return new OrderEntity
            {
                CustomerID = order.CustomerID,
                TableID = order.TableID,
                StatusName = string.Empty,
                OrderTypeName = string.Empty,
                CreatedByUserID = order.CreatedByUserID,
                OrderStatus = OrderEntity.enOrderStatus.Pending, // Default status for new orders
                OrderType = (OrderEntity.enOrderType)order.OrderType, // Map the order type from DTO to entity
                Username = string.Empty,
                TotalPrice = 0, // Total price will be calculated Internally In The DB based on the items in the order
                CreatedAt = DateTime.UtcNow,
                notes = order.Notes
            };
        }

        public static void UpdateOrderEntity(OrderEntity existingOrder, UpdateOrderDTO updatedOrder)
        {
            existingOrder.TableID = updatedOrder.TableID ?? existingOrder.TableID;
            existingOrder.OrderStatus = updatedOrder.OrderStatus.HasValue ? (OrderEntity.enOrderStatus)updatedOrder.OrderStatus.Value : existingOrder.OrderStatus;
            existingOrder.notes = updatedOrder.Notes;
        }

        public static DataTable ConvertToDataTable<T>(List<T> data)
        {
            DataTable table = new DataTable(typeof(T).Name);

            // Get all properties of T
            var properties = typeof(T).GetProperties();

            // Create columns
            foreach (var prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Fill rows
            foreach (var item in data)
            {
                var values = new object[properties.Length];

                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(values);
            }

            return table;
        }
    }
}