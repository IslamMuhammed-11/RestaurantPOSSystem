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
                        Role = (string)reader["Role"],
                        UserName = (string)reader["Username"],
                        PasswordHash = (string)reader["PasswordHash"],
                        IsActive = (bool)reader["IsActive"],
                        RefreshTokenHash = reader["RefreshTokenHash"] == DBNull.Value ? null : (string)reader["RefreshTokenHash"],
                        ExpiresAt = reader["ExpiresAt"] == DBNull.Value ? null : (DateTime)reader["ExpiresAt"],
                        RevokedAt = reader["RevokedAt"] == DBNull.Value ? null : (DateTime)reader["RevokedAt"]
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

        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetUserByUsername", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", username);

            try
            {
                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserEntity
                    {
                        UserID = (int)reader["UserID"],
                        PersonID = (int)reader["PersonID"],
                        RoleID = (int)reader["RoleID"],
                        Role = (string)reader["Role"],
                        UserName = (string)reader["Username"],
                        PasswordHash = (string)reader["PasswordHash"],
                        IsActive = (bool)reader["IsActive"],
                        RefreshTokenHash = reader["RefreshTokenHash"] == DBNull.Value ? null : (string)reader["RefreshTokenHash"],
                        ExpiresAt = reader["ExpiresAt"] == DBNull.Value ? null : (DateTime?)reader["ExpiresAt"],
                        RevokedAt = reader["RevokedAt"] == DBNull.Value ? null : (DateTime?)reader["RevokedAt"]
                    };
                }

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
                        Role = (string)reader["Role"],
                        UserName = (string)reader["UserName"],
                        PasswordHash = (string)reader["PasswordHash"],
                        IsActive = (bool)reader["IsActive"],
                        RefreshTokenHash = reader["RefreshTokenHash"] == DBNull.Value ? null : (string)reader["RefreshTokenHash"],
                        ExpiresAt = reader["ExpiresAt"] == DBNull.Value ? null : (DateTime?)reader["ExpiresAt"],
                        RevokedAt = reader["RevokedAt"] == DBNull.Value ? null : (DateTime?)reader["RevokedAt"]
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

            if (user.RefreshTokenHash == null)
                cmd.Parameters.AddWithValue("@RefreshTokenHash", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@RefreshTokenHash", user.RefreshTokenHash);

            if (user.ExpiresAt == null)
                cmd.Parameters.AddWithValue("@ExpiresAt", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@ExpiresAt", user.ExpiresAt);

            if (user.RevokedAt == null)
                cmd.Parameters.AddWithValue("@RevokedAt", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@RevokedAt", user.RevokedAt);

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

        public async Task<bool> SaveRefreshTokenAsync(UserEntity user)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_LoginUser", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", user.UserID);
            cmd.Parameters.AddWithValue("@RefreshTokenHash", user.RefreshTokenHash);
            cmd.Parameters.AddWithValue("@ExpiresAt", user.ExpiresAt);

            int RowsAffected = 0;
            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (SqlException)
            {
                return false;
            }

            return RowsAffected > 0;
        }

        public async Task<bool> RevokeToken(int id)
        {
            using SqlConnection connection = new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_RevokeUserToken", connection);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", id);

            int RowsAffected = 0;

            try
            {
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();
                if (result != null && int.TryParse(result.ToString(), out int num))
                    RowsAffected = num;
            }
            catch (Exception)
            {
                return false;
            }

            return RowsAffected > 0;
        }
    }
}