
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
    public class TableRepo : ITableRepo
    {
        private readonly string _ConnString;

        public TableRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<List<TableEntity>> GetAllTablesAsync()
        {
            List<TableEntity> tables = new List<TableEntity>();
            try
            {
                using SqlConnection connection = new SqlConnection(_ConnString);
                using SqlCommand cmd = new SqlCommand("SP_GetAllTables", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tables.Add(new TableEntity
                    {
                        TableID = (int)reader["TableID"],
                        TableStatus = reader["TableStatus"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TableStatus"]),
                        NumberOfSeats = reader["Seats"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["Seats"])
                    });
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.ActionResultEnum.ActionResult.DBError);
            }

            return tables;
        }

        public async Task<TableEntity> GetTableByIdAsync(int tableId)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetTableByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TableID", tableId);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TableEntity
                    {
                        TableID = (int)reader["TableID"],
                        TableStatus = reader["TableStatus"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TableStatus"]),
                        NumberOfSeats = reader["Seats"] == DBNull.Value ? (short)0 : Convert.ToInt16(reader["Seats"])
                    };
                }
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.ActionResultEnum.ActionResult.DBError);
            }

            return null!;
        }

        public async Task<int?> CreateTableAsync(TableEntity table)
        {
            if (table == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewTable", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Seats", table.NumberOfSeats);

            SqlParameter param = new SqlParameter("@TableID", SqlDbType.Int)
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
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.ActionResultEnum.ActionResult.DBError);
            }

            if (param.Value == DBNull.Value)
                return null;

            return (int?)param.Value;
        }

        public async Task<bool> UpdateTableAsync(TableEntity table)
        {
            if (table == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateTable", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Seats", table.NumberOfSeats);
            cmd.Parameters.AddWithValue("@TableID", table.TableID);

            try
            {
                await connection.OpenAsync();
                object? result = await cmd.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.ActionResultEnum.ActionResult.DBError);
            }

            return RowsAffected > 0;
        }

        public async Task<bool> DeleteTableAsync(int tableId)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteTable", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TableID", tableId);

            try
            {
                await connection.OpenAsync();
                object? result = await cmd.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message, 99999, Contracts.Enums.ActionResultEnum.ActionResult.DBError);
            }

            return RowsAffected > 0;
        }
    }
}
