using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Entites;
using Contracts.Exceptions;

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

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ItemsEntity
                    {
                        ItemID = (int)reader["ItemID"],
                        ProductID = (int)reader["ProductID"],
                        OrderID = (int)reader["OrderID"],
                        Quantity = reader["Quantity"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["Quantity"]),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
            }

            return null!;
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
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
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
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
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
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
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
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
            }

            return RowsAffected > 0;
        }
    }
}



    

