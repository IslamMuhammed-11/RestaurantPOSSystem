using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccessLayer.Repos
{
    public class RefundedPaymentsRep : IRefundedPaymentsRepo
    {
        private readonly string _ConnString;

        public RefundedPaymentsRep(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<int?> RefundPaymentAsync(RefundedPaymentsEntity refund)
        {
            if (refund == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_RefundPayment", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PaymentID", refund.PaymentID);
            cmd.Parameters.AddWithValue("@Reason", refund.Reason);
            cmd.Parameters.AddWithValue("@Amount", refund.Amount);

            SqlParameter outputParam = new SqlParameter("@RefundID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outputParam);

            try
            {
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                // DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            if (outputParam.Value == DBNull.Value)
                return null;

            return (int?)outputParam.Value;
        }

        public async Task<List<RefundedPaymentsEntity>> GetAllRefundedPaymentsAsync()
        {
            List<RefundedPaymentsEntity> refunds = new List<RefundedPaymentsEntity>();

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetAllRefundedPayments", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    refunds.Add(new RefundedPaymentsEntity
                    {
                        RefundID = (int)reader["RefundID"],
                        PaymentID = (int)reader["PaymentID"],
                        Reason = reader["Reason"]?.ToString() ?? string.Empty,
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        RefundedAt = Convert.ToDateTime(reader["RefundedAt"])
                    });
                }
            }
            catch (SqlException ex)
            {
                //DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            return refunds;
        }

        public async Task<RefundedPaymentsEntity?> GetRefundedPaymentByIDAsync(int refundId)
        {
            if (refundId <= 0)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetRefundedPaymentByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", refundId);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RefundedPaymentsEntity
                    {
                        RefundID = (int)reader["RefundID"],
                        PaymentID = (int)reader["PaymentID"],
                        Reason = reader["Reason"]?.ToString() ?? string.Empty,
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        RefundedAt = Convert.ToDateTime(reader["RefundedAt"])
                    };
                }
            }
            catch (SqlException ex)
            {
                //DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw new BusinessException(ex.Message, 99999, ActionResultEnum.ActionResult.DBError);
            }

            return null;
        }
    }
}