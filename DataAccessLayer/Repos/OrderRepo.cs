using Contracts.Enums;
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

        public async Task<OrderEntity?> GetOrderByIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetOrderByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = id;
            try
            {
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
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);

                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return null;
        }

        public async Task<List<OrderEntity>> GetAllOrdersAsync()
        {
            List<OrderEntity> orders = new List<OrderEntity>();
            try
            {
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
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
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
                    50001 => new BusinessException("Invalid ProductID", 50001, Enums.ActionResult.InvalidData),
                    50002 => new BusinessException("Product Isn't Available", 50002, Enums.ActionResult.Conflict),
                    50003 => new BusinessException("No Order Items Sent!", 50003, Enums.ActionResult.InvalidData),
                    50004 => new BusinessException("Invalid Quantity", 50004, Enums.ActionResult.InvalidData),
                    50005 => new BusinessException("Table not available", 50005, Enums.ActionResult.Conflict),
                    50006 => new BusinessException("Invalid TableID", 50006, Enums.ActionResult.InvalidData),
                    50007 => new BusinessException("Invalid Order Type", 50007, Enums.ActionResult.InvalidData),
                    50009 => new BusinessException("TableID is required for DineIn", 50009, Enums.ActionResult.InvalidData),

                    _ => new BusinessException("Database error occurred", 99999, Enums.ActionResult.DBError)
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
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
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
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
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
                //DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }
            return RowsAffected > 0;
        }
    }
}