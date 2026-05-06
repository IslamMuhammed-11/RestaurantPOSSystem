using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Repos
{
    public class ProductRepo : IProductRepo
    {
        private readonly string _ConnString;

        public ProductRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<ProductEntity?> GetProductByIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetProductByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ProductEntity
                {
                    ProductID = (int)reader["ProductID"],
                    CategoryID = (int)reader["CategoryID"],
                    Name = reader["Name"]?.ToString() ?? string.Empty,
                    Price = Convert.ToDecimal(reader["Price"]),
                    IsAvailable = reader["IsAvailable"] != DBNull.Value && (bool)reader["IsAvailable"]
                };
            }
            return null;
        }

        public async Task<List<ProductEntity>> GetAllProductsAsync()
        {
            List<ProductEntity> products = new List<ProductEntity>();

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetAllProducts", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new ProductEntity
                {
                    ProductID = (int)reader["ProductID"],
                    CategoryID = (int)reader["CategoryID"],
                    Name = reader["Name"]?.ToString() ?? string.Empty,
                    Price = Convert.ToDecimal(reader["Price"]),
                    IsAvailable = reader["IsAvailable"] != DBNull.Value && (bool)reader["IsAvailable"]
                });
            }

            return products;
        }

        public async Task<int?> CreateProductAsync(ProductEntity product)
        {
            if (product == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewProduct", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@IsAvailable", product.IsAvailable);

            SqlParameter param = new SqlParameter("@ProductID", SqlDbType.Int)
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

        public async Task<bool> UpdateProductAsync(ProductEntity product)
        {
            if (product == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateProduct", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@IsAvailable", product.IsAvailable);
            cmd.Parameters.AddWithValue("@ProductID", product.ProductID);

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

        public async Task<bool> DeleteProductAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteProduct", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", id);

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

        public async Task<bool> DoesProductExistAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DoesProductExist", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", id);

            await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
                return true;

            return false;
        }

        public async Task<bool> IsProductAvailableAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_IsProductAvailable", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", id);

            await connection.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
                return true;

            return false;
        }

        public async Task<List<int>> ValidateProducts(DataTable products)
        {
            List<int> InvalidIDs = new List<int>();
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_ValidateProducts", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ProductsIds", SqlDbType.Structured).Value = products;

            await connection.OpenAsync();
            using SqlDataReader InvalidData = await cmd.ExecuteReaderAsync();

            while (await InvalidData.ReadAsync())
            {
                int id = (int)InvalidData["ProductID"];
                InvalidIDs.Add(id);
            }

            return InvalidIDs;
        }
    }
}