using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using DataAccessLayer.Entites;
using System.Diagnostics;
namespace DataAccessLayer.Repos
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly string _ConnString;

        public CustomerRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<CustomerEntity?> GetCustomerByIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetCustomerByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerID", id);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new CustomerEntity
                    {
                        CustomerID = (int)reader["CustomerID"],
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Phone = reader["Phone"]?.ToString()
                    };
                }

                
            }
            catch (SqlException ex)
            {

                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return null;
        }

        public async Task<List<CustomerEntity>> GetAllCustomersAsync()
        {
            List<CustomerEntity> customers = new List<CustomerEntity>();
            try
            {
                using SqlConnection connection = new SqlConnection(_ConnString);
                using SqlCommand cmd = new SqlCommand("SP_GetAllCustomers", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    customers.Add(new CustomerEntity
                    {
                        CustomerID = (int)reader["CustomerID"],
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Phone = reader["Phone"]?.ToString()
                    });
                }
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return customers;
        }

        public async Task<int?> AddNewCustomerAsync(CustomerEntity customer)
        {
            if (customer == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewCustomer", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);

            SqlParameter param = new SqlParameter("@CustomerID", SqlDbType.Int)
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
            }

            if (param.Value == DBNull.Value)
                return null;

            return (int?)param.Value;
        }

        public async Task<bool> UpdateCustomerAsync(CustomerEntity customer)
        {
            if (customer.CustomerID <= 0)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateCustomer", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);

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
                return false;
            }

            return RowsAffected > 0;
        }

        public async Task<bool> DeleteCustomerAsync(int CustomerID)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteCustomer", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

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
                return false;
            }

            return RowsAffected > 0;
        }
    }
}
