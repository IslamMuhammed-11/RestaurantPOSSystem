using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class DailySalesRepo : IDailySalesRepo
    {
        private readonly string _ConnString;

        public DailySalesRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<bool> LogDailySales(decimal amount)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_LogDailySales", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = amount;

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
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return RowsAffected > 0;
        }

        //-------------This is a ProductSalesRepo Method-------------//
        public async Task<bool> LogProductSales(int orderId)
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
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return RowsAffected > 0;
        }

        public async Task<SalesComparisonEntity> SalesComparison(DateOnly currentStart, DateOnly currentEnd,
                                                                    DateOnly prevStart, DateOnly prevEnd)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_SalesComparison", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@CurrentStart", SqlDbType.Date).Value = currentStart;
            cmd.Parameters.Add("@CurrentEnd", SqlDbType.Date).Value = currentEnd;
            cmd.Parameters.Add("@PrevStart", SqlDbType.Date).Value = prevStart;
            cmd.Parameters.Add("@PrevEnd", SqlDbType.Date).Value = prevEnd;

            SalesComparisonEntity result = new();
            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.Read())
                {
                    result.Current = new DailySalesEntity
                    {
                        TotalOrders = (int)reader["TotalOrders"],
                        Gross = Convert.ToDecimal(reader["TotalGross"])
                    };

                    await reader.NextResultAsync();

                    result.Prev = new DailySalesEntity
                    {
                        TotalOrders = (int)reader["TotalOrders"],
                        Gross = Convert.ToDecimal(reader["TotalGross"])
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return result;
        }

        public async Task<DailySalesEntity?> SalesDetails(DateOnly from, DateOnly to)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_SalesDetails", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@From", SqlDbType.Date).Value = from;
            cmd.Parameters.Add("@To", SqlDbType.Date).Value = to;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.Read())
                {
                    return new DailySalesEntity
                    {
                        TotalOrders = (int)reader["TotalOrders"],
                        Gross = Convert.ToDecimal(reader["TotalGross"]),
                        Net = Convert.ToDecimal(reader["TotalNet"])
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return null;
        }

        public async Task<List<DailySalesEntity>> SalesTrend(DateOnly from, DateOnly to)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_SalesDetails", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@From", SqlDbType.Date).Value = from;
            cmd.Parameters.Add("@To", SqlDbType.Date).Value = to;

            List<DailySalesEntity> list = new();

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (reader.Read())
                {
                    list.Add(new DailySalesEntity
                    {
                        Date = (DateOnly)reader["Date"],
                        Gross = Convert.ToDecimal(reader["TotalSales"])
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return list;
        }
    }
}