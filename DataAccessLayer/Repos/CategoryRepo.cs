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
    public class CategoryRepo : ICategoryRepo
    {
        private readonly string _ConnString;

        public CategoryRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<CategoryEntity?> GetCategoryByIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetCategoryByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CategoryID", id);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new CategoryEntity
                    {
                        CategoryID = (int)reader["CategoryID"],
                        Name = reader["Name"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
            }
            return null;
        }

        public async Task<List<CategoryEntity>> GetAllCategoriesAsync()
        {
            List<CategoryEntity> categories = new List<CategoryEntity>();
            try
            {
                using SqlConnection connection = new SqlConnection(_ConnString);
                using SqlCommand cmd = new SqlCommand("SP_GetAllCategories", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    categories.Add(new CategoryEntity
                    {
                        CategoryID = (int)reader["CategoryID"],
                        Name = reader["Name"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
            }

            return categories;
        }

        public async Task<int?> CreateCategoryAsync(CategoryEntity category)
        {
            if (category == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewCategory", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", category.Name);

            SqlParameter param = new SqlParameter("@CategoryID", SqlDbType.Int)
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

        public async Task<bool> UpdateCategoryAsync(CategoryEntity category)
        {
            if (category == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateCategory", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", category.Name);
            cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);

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

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteCategory", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CategoryID", id);

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

        public async Task<bool> DoesCategoryExistAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DoesCategoryExist", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CategoryID", id);
            try
            {
                await connection.OpenAsync();

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                    return true;

            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.Enums.ActionResult.DBError);
            }
            return false;
        }
    }
}
