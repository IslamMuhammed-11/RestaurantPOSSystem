using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Repos
{
    public class ItemRepo : IItemRepo
    {
        private readonly string _ConnString;

        public ItemRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<ItemsEntity> GetItemByIdAsync(int Id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetItemById", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ItemID", Id);

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemsEntity
                {
                    ItemID = (int)reader["ItemID"],
                    ProductID = (int)reader["ProductID"],
                    ProductName = (string)reader["ProductName"],
                    OrderID = (int)reader["OrderID"],
                    Quantity = reader["Quantity"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["Quantity"]),
                    Price = Convert.ToDecimal(reader["UnitPrice"])
                };
            }

            return null!;
        }

        public async Task<List<ItemsEntity>> GetAllItemsByOrderId(int orderId)
        {
            List<ItemsEntity> items = new List<ItemsEntity>();

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetAllItemsByOrderId", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderID", orderId);

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                items.Add(new ItemsEntity
                {
                    ItemID = (int)reader["ItemID"],
                    ProductID = (int)reader["ProductID"],
                    ProductName = (string)reader["ProductName"],
                    OrderID = (int)reader["OrderID"],
                    Quantity = reader["Quantity"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["Quantity"]),
                    Price = Convert.ToDecimal(reader["UnitPrice"])
                });
            }

            return items;
        }

        public async Task<int?> AddNewItemAsync(ItemsEntity item)
        {
            if (item == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewItem", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
            cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            // cmd.Parameters.AddWithValue("@Price", item.Price);

            SqlParameter param = new SqlParameter("@ItemID", SqlDbType.Int)
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

        public async Task<bool> UpdateItemsAsync(ItemsEntity item)
        {
            if (item == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateItem", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
            cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@ItemID", item.ItemID);

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

        public async Task<bool> UpdateQuantityAsync(ItemsEntity item)
        {
            if (item == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateItemQuantity", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@OrderID", item.OrderID);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@ItemID", item.ItemID);

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

        public async Task<bool> DeleteItemsAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteItem", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ItemID", id);

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