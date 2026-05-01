using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using DataAccessLayer.Entites;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace DataAccessLayer.Repos
{
    public class RolesRepo : IRoleRepo
    {
        private readonly string _ConnString;

        public RolesRepo(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<RoleEntity?> GetRoleByIDAsync(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetRoleByID", connection);

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleID", id);

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new RoleEntity
                    {
                        RoleID = (int)reader["RoleID"],
                        RoleName = reader["RoleName"]?.ToString() ?? string.Empty,
                        // Premission = Convert.ToInt16(reader["Premission"])
                    };
                }
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, EventLogEntryType.Error);
            }

            return null;
        }

        public async Task<List<RoleEntity>> GetAllRoleAsync()
        {
            List<RoleEntity> roles = new List<RoleEntity>();
            try
            {
                using SqlConnection connection = new SqlConnection(_ConnString);
                using SqlCommand cmd = new SqlCommand("SP_GetAllRoles", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    roles.Add(new RoleEntity
                    {
                        RoleID = (int)reader["RoleID"],
                        RoleName = reader["RoleName"]?.ToString() ?? string.Empty,
                        //Premission = Convert.ToInt16(reader["Permission"])
                    });
                }
            }
            catch (Exception ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return roles;
        }

        public async Task<int?> AddNewRoleAsync(RoleEntity role)
        {
            if (role == null)
                return null;

            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewRole", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
            //cmd.Parameters.AddWithValue("@Premission", role.Premission);

            SqlParameter param = new SqlParameter("@RoleID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(param);

            try
            {
                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            if (param.Value == DBNull.Value)
                return null;

            return (int?)param.Value;
        }

        public async Task<bool> UpdateRoleAsync(RoleEntity role)
        {
            if (role == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateRole", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RoleName", role.RoleName);
            cmd.Parameters.AddWithValue("@Premission", role.Premission);
            cmd.Parameters.AddWithValue("@RoleID", role.RoleID);

            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (Exception ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

            return RowsAffected > 0;
        }

        public async Task<bool> DeleteRoleAsync(int roleID)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteRole", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RoleID", roleID);

            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (Exception ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

            return RowsAffected > 0;
        }
    }
}