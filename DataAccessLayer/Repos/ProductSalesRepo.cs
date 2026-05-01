using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Repos
{
    public class ProductSalesRepo : IProductSalesRepo
    {
        private readonly string _ConnString;

        public ProductSalesRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new BusinessException("Connection string not found.", 99999, ActionResultEnum.ActionResult.Error);
        }

        public async Task<bool> LogProductSalesAsync(int orderId)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_LogProductSales", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderId;

            int RowsAffected = 0;

            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int ID))
                    RowsAffected = ID;
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            return RowsAffected > 0;
        }

        public async Task<List<ProductSalesEntity>> TopFiveProductsInPeriodAsync(DateOnly from, DateOnly to)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_TopFiveProductsInPeriod", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@From", SqlDbType.Date).Value = from;
            cmd.Parameters.Add("@To", SqlDbType.Date).Value = to;

            List<ProductSalesEntity> list = new();

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    list.Add(new ProductSalesEntity()
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = (string)reader["Name"],
                        QuantitySold = (int)reader["TotalSold"],
                        TotalRevenue = (decimal)reader["TotalRevenue"]
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            return list;
        }
    }
}