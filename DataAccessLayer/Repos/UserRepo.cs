using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccessLayer.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly string _ConnString;

        public UserRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async Task<UserEntity?> GetUserByIDAsync(int ID)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetUserByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", ID);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserEntity
                    {
                        UserID = ID,
                        PersonID = (int)reader["PersonID"],
                        RoleID = (int)reader["RoleID"],
                        UserName = (string)reader["Username"],
                        PasswordHash = (string)reader["PasswordHash"],
                        IsActive = (bool)reader["IsActive"]
                    };
                }
                else
                    return null;
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return null;
            }
        }

        public async Task<List<UserEntity>> GetAllUserAsync()
        {
            List<UserEntity> users = new List<UserEntity>();
            try
            {
                using SqlConnection connection =
                    new SqlConnection(_ConnString);
                using SqlCommand cmd = new SqlCommand("SP_GetAllUsers", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new UserEntity
                    {
                        UserID = (int)reader["UserID"],
                        PersonID = (int)reader["PersonID"],
                        RoleID = (int)reader["RoleID"],
                        UserName = (string)reader["UserName"],
                        PasswordHash = (string)reader["PasswordHash"],
                        IsActive = (bool)reader["IsActive"]
                    });
                }
            }
            catch (Exception ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return users;
        }

        public async Task<int?> AddNewUserAsync(UserEntity user)
        {
            if (user == null)
                return null;

            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewUser", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", user.PersonID);
            cmd.Parameters.AddWithValue("@RoleID", user.RoleID);
            cmd.Parameters.AddWithValue("@Username", user.UserName);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            SqlParameter param = new SqlParameter("@UserID", SqlDbType.Int)
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

            return (int?)param.Value;
        }

        public async Task<bool> UpdateUserAsync(UserEntity user)
        {
            if (user == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdateUser", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", user.PersonID);
            cmd.Parameters.AddWithValue("@RoleID", user.RoleID);
            cmd.Parameters.AddWithValue("@Username", user.UserName);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
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

        public async Task<bool> DeleteUserAsync(int UserID)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeleteUser", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", UserID);

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

        public async Task<bool> DeactivateUserAsync(int UserID)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);

            using SqlCommand cmd = new SqlCommand("SP_DeactivateUser", connection);

            int RowsAffected = 0;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);
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

        public async Task<bool> ActivateUserAsync(int UserID)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);

            int RowsAffected = 0;
            using SqlCommand cmd = new SqlCommand("SP_ActivateUser", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);

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

        public async Task<bool> UpdatePasswordAsync(int UserID, string NewPasswordHash)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);

            int RowsAffected = 0;
            using SqlCommand cmd = new SqlCommand("SP_UpdateUserPassword", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@NewPasswordHash", NewPasswordHash);

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

        public async Task<bool> DoesUserExistAsync(int UserID)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DoesUserExist", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                await connection.OpenAsync();
                object? result = await cmd.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int num))
                    return num > 0;
                else
                    return false;
            }
            catch (SqlException ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }
        }
    }
}