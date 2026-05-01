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
    public class PaymentRepo : IPaymentRepo
    {
        private readonly string _ConnString;

        public PaymentRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<int?> CreateNewPaymentAsync(PaymentEntity entity)
        {
            if (entity == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewPayment", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderID", SqlDbType.Int).Value = entity.OrderID;
            cmd.Parameters.Add("@PaymentMethodID", SqlDbType.Int).Value = entity.PaymentMethodID;
            cmd.Parameters.Add("@PaymentAmount", SqlDbType.Decimal).Value = entity.PaidAmount;

            SqlParameter param = new SqlParameter("@PaymentID", SqlDbType.Int)
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
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            if (param.Value == DBNull.Value)
                return null;

            return (int?)param.Value;
        }

        public async Task<List<PaymentEntity>> GetAllPaymentsAsync()
        {
            List<PaymentEntity> payments = new List<PaymentEntity>();

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetAllPayments", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    payments.Add(new PaymentEntity
                    {
                        PaymentID = (int)reader["PaymentID"],
                        OrderID = (int)reader["OrderID"],
                        PaymentMethodID = (int)reader["PaymentMethod"],
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"])
                    });
                }
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            return payments;
        }

        public async Task<PaymentEntity?> GetPaymentByPaymentIdAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetPaymentByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PaymentID", SqlDbType.Int).Value = id;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new PaymentEntity
                    {
                        PaymentID = (int)reader["PaymentID"],
                        OrderID = (int)reader["OrderID"],
                        PaymentMethodID = (int)reader["PaymentMethod"],
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"])
                    };
                }
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            return null;
        }

        public async Task<PaymentEntity?> GetPaymentByOrderIdAsync(int orderId)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetPaymentByOrderID" , connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderID" , SqlDbType.Int).Value = orderId;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = cmd.ExecuteReader();

                if (await reader.ReadAsync())
                {
                    return new PaymentEntity
                    {
                        PaymentID = (int)reader["PaymentID"],
                        OrderID = (int)reader["OrderID"],
                        PaymentMethodID = (int)reader["PaymentMethod"],
                        PaymentDate = (DateTime)reader["PaymentDate"],
                        PaidAmount = (decimal)reader["PaidAmount"]
                    };
                }
            }
            catch (SqlException ex)
            {

                throw new BusinessException(ex.Message, 80000, ActionResultEnum.ActionResult.DBError);
            }

            return null;
        }

        public async Task<bool> IsPaid(int orderId)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_IsPaid" , connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@OrderID" , SqlDbType.Int).Value= orderId;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                    return true;
                else 
                    return false;
            }
            catch (SqlException ex)
            {
                throw new BusinessException(ex.Message , 80000 , ActionResultEnum.ActionResult.DBError); 
            }

        }


    }
}
