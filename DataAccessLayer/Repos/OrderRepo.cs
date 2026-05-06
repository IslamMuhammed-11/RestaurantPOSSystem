using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Repos
{
    public class OrderRepo : IOrderRepo
    {
        private readonly string _ConnString;

        public OrderRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<OrderAndItemsEntity?> GetOrderAndItemsByOrderIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetOrderAndItemsByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = id;
            OrderEntity order;
            List<ItemsEntity> items = new List<ItemsEntity>();

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                order = new OrderEntity
                {
                    OrderID = (int)reader["OrderID"],

                    CustomerID = reader["CustomerID"] == DBNull.Value ? null : (int?)reader["CustomerID"],

                    CreatedByUserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString() ?? string.Empty,

                    TableID = reader["TableID"] == DBNull.Value ? null : (int?)reader["TableID"],

                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),

                    OrderStatus = (OrderEntity.enOrderStatus)Convert.ToInt32(reader["OrderStatus"]),
                    StatusName = reader["StatusName"].ToString() ?? string.Empty,

                    OrderType = (OrderEntity.enOrderType)Convert.ToInt32(reader["OrderType"]),
                    OrderTypeName = reader["OrderTypeName"].ToString() ?? string.Empty,

                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["UpdatedAt"]),

                    notes = reader["Notes"]?.ToString()
                };

                await reader.NextResultAsync();

                while (reader.Read())
                {
                    items.Add(new ItemsEntity
                    {
                        ItemID = (int)reader["ItemID"],
                        OrderID = id,
                        Price = (decimal)reader["UnitPrice"],
                        ProductID = (int)reader["ProductID"],
                        ProductName = (string)reader["ProductName"],
                        Quantity = (byte)reader["Quantity"]
                    });
                }
            }
            else
                return null;

            return new OrderAndItemsEntity
            {
                Order = order,
                Items = items
            };
        }

        public async Task<OrderEntity?> GetOrderByIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetOrderByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OrderEntity
                {
                    OrderID = (int)reader["OrderID"],

                    CustomerID = reader["CustomerID"] == DBNull.Value ? null : (int?)reader["CustomerID"],

                    CreatedByUserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString() ?? string.Empty,

                    TableID = reader["TableID"] == DBNull.Value ? null : (int?)reader["TableID"],

                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),

                    OrderStatus = (OrderEntity.enOrderStatus)Convert.ToInt32(reader["OrderStatus"]),
                    StatusName = reader["StatusName"].ToString() ?? string.Empty,

                    OrderType = (OrderEntity.enOrderType)Convert.ToInt32(reader["OrderType"]),
                    OrderTypeName = reader["OrderTypeName"].ToString() ?? string.Empty,

                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["UpdatedAt"]),

                    notes = reader["Notes"]?.ToString()
                };
            }

            return null;
        }

        public async Task<List<OrderEntity>> GetAllOrdersAsync()
        {
            List<OrderEntity> orders = new List<OrderEntity>();

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetAllOrders", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new OrderEntity
                {
                    OrderID = (int)reader["OrderID"],

                    CustomerID = reader["CustomerID"] == DBNull.Value ? null : (int?)reader["CustomerID"],

                    CreatedByUserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString() ?? string.Empty,

                    TableID = reader["TableID"] == DBNull.Value ? null : (int?)reader["TableID"],

                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),

                    OrderStatus = (OrderEntity.enOrderStatus)Convert.ToInt32(reader["OrderStatus"]),
                    StatusName = reader["StatusName"].ToString() ?? string.Empty,

                    OrderType = (OrderEntity.enOrderType)Convert.ToInt32(reader["OrderType"]),
                    OrderTypeName = reader["OrderTypeName"].ToString() ?? string.Empty,

                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["UpdatedAt"]),

                    notes = reader["Notes"]?.ToString()
                });
            }

            return orders;
        }

        public async Task<int?> CreateOrderAsync(OrderEntity order, DataTable orderItems)
        {
            if (order == null || orderItems == null)
                throw new ArgumentNullException("Order Or OrderItems Are Null");

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_CreateOrder", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = (object?)order.CustomerID ?? DBNull.Value;
            cmd.Parameters.Add("@TableID", SqlDbType.Int).Value = (object?)order.TableID ?? DBNull.Value;
            cmd.Parameters.Add("@CreatedByUserID", SqlDbType.Int).Value = order.CreatedByUserID;
            cmd.Parameters.Add("@OrderType", SqlDbType.Int).Value = (int)order.OrderType;
            cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object?)order.notes ?? DBNull.Value;
            cmd.Parameters.Add("@OrderItemType", SqlDbType.Structured).Value = orderItems;

            SqlParameter param = new SqlParameter("@OrderID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(param);

            try
            {
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 or 2601 => new DuplicateRecordException(),
                    547 => new InvalidReferenceTypeException(),
                    _ => ex
                };
            }
            if (param.Value == DBNull.Value)
                return null;

            return (int?)param.Value;
        }

        public async Task<bool> UpdateOrderAsync(OrderEntity order)
        {
            if (order == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateOrder", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@TableID", SqlDbType.Int).Value = (object?)order.TableID ?? DBNull.Value;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.Int).Value = (int)order.OrderStatus;
            cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object?)order.notes ?? DBNull.Value;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = order.OrderID;

            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 or 2601 => new DuplicateRecordException(),
                    547 => new InvalidReferenceTypeException(),
                    _ => ex
                };
            }

            return RowsAffected > 0;
        }

        public async Task<bool> ChangeOrderStatus(int id, OrderEntity.enOrderStatus orderStatus)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid order ID.");

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateOrderStatus", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@OrderStatus", SqlDbType.Int).Value = (int)orderStatus;

            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 or 2601 => new DuplicateRecordException(),
                    547 => new InvalidReferenceTypeException(),
                    _ => ex
                };
            }

            return RowsAffected > 0;
        }

        public async Task<bool> ChangeTable(int id, int TableID)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid order ID.");
            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_ChangeOrderTable", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@TableID", SqlDbType.Int).Value = TableID;
            try
            {
                await connection.OpenAsync();
                object? result = await cmd.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 or 2601 => new DuplicateRecordException(),
                    547 => new InvalidReferenceTypeException(),
                    _ => ex
                };
            }
            return RowsAffected > 0;
        }
    }
}