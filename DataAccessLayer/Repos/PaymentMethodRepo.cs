using DataAccessLayer.Interfaces;
using DataAccessLayer.Entites;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Contracts.Exceptions;
using Contracts.Enums;

namespace DataAccessLayer.Repos
{
    public class PaymentMethodRepo : IPaymentMethodRepo
    {
        private readonly string _ConnString;

        public PaymentMethodRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<int?> AddNewMethod(PaymentMethodEntity method)
        {
            if (method == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewPaymentMethod", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PaymentMethodName", method.PaymentMethod);

            SqlParameter param = new SqlParameter("@PaymentMethodID", SqlDbType.Int)
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
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            if (param.Value == DBNull.Value)
                return null;

            return (int?)param.Value;
        }

        public async Task<List<PaymentMethodEntity>> GetAllMethods()
        {
            List<PaymentMethodEntity> methods = new List<PaymentMethodEntity>();

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetAllPaymentMethods", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    methods.Add(new PaymentMethodEntity
                    {
                        MethodID = (int)reader["MethodID"],
                        PaymentMethod = reader["MethodName"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, Enums.ActionResult.DBError);
            }

            return methods;
        }

        public async Task<PaymentMethodEntity?> GetMethodByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetPaymentMethodByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PaymentMethodID", id);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new PaymentMethodEntity
                    {
                        MethodID = (int)reader["MethodID"],
                        PaymentMethod = reader["MethodName"]?.ToString() ?? string.Empty
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

        public async Task<bool> DeleteMethod(int methodId)
        {
            if (methodId <= 0)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeletePaymentMethod", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PaymentMethodID", methodId);

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

        public async Task<bool> UpdateMethod(PaymentMethodEntity method)
        {
            if (method == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdatePaymentMethod", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PaymentMethodName", method.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentMethodID", method.MethodID);

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
    }
}
