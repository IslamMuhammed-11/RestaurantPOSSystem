using DataAccessLayer.Entites;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Interfaces;
namespace DataAccessLayer.Repos
{
    public class PersonRepo : IPersonRepo
    {
        private readonly string _ConnString;

        public PersonRepo(IConfiguration configuration)
        {
            _ConnString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException();
        }

        public async  Task<PersonEntity?> GetPersonByIDAsync(int id)
        {
            using SqlConnection connection =
                new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_GetPersonByID", connection);

            cmd.CommandType = CommandType.StoredProcedure;

           cmd.Parameters.AddWithValue("@PersonID", id);

           await connection.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new PersonEntity
                {
                    PersonID = id,
                    FirstName = (string)reader["FirstName"],
                    SecondName = (string)reader["SecondName"],
                    ThirdName = (string)reader["ThirdName"],
                    DateOfBirth = (DateTime)reader["DateOfBirth"],
                    Gender = (bool)reader["Gender"],
                    Phone = (string)reader["Phone"],
                    Address = (string)reader["Address"]
                };
            }
            else
                return null;
        }

        public async  Task<List<PersonEntity>> GetAllPeopleAsync()
        {
            List<PersonEntity> People = new List<PersonEntity>();
            try
            {
                using SqlConnection connection =
             new SqlConnection(_ConnString);
                using SqlCommand cmd = new SqlCommand("SP_GetAllPeople", connection);

                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    People.Add(new PersonEntity
                    {
                        PersonID = (int)reader["PersonID"],
                        FirstName = (string)reader["FirstName"],
                        SecondName = (string)reader["SecondName"],
                        ThirdName = (string)reader["ThirdName"],
                        DateOfBirth = (DateTime)reader["DateOfBirth"],
                        Gender = (bool)reader["Gender"],
                        Phone = (string)reader["Phone"],
                        Address = (string)reader["Address"]
                    });
                }
            }
            catch (Exception ex)
            {
                DataAccessSettings.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return People;
        }

        public async  Task<int?> AddNewPersonAsync(PersonEntity person)
        {
            if (person == null)
                return null;            
            
            using SqlConnection connection =
            new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_AddNewPerson", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
            cmd.Parameters.AddWithValue("@SecondName", person.SecondName);
            cmd.Parameters.AddWithValue("@ThirdName", person.ThirdName);
            cmd.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
            cmd.Parameters.AddWithValue("@Phone", person.Phone);
            cmd.Parameters.AddWithValue("@Address", person.Address);
            cmd.Parameters.AddWithValue("@Gender", person.Gender);
            SqlParameter param = new  SqlParameter("@PersonID", SqlDbType.Int)
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

            return (int)param.Value;
        }

        public async  Task<bool> UpdatePersonAsync(PersonEntity person)
        {
            if (person == null)
                return false;

            int RowsAffected = 0;
            using SqlConnection connection =
            new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_UpdatePerson", connection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
            cmd.Parameters.AddWithValue("@SecondName", person.SecondName);
            cmd.Parameters.AddWithValue("@ThirdName", person.ThirdName);
            cmd.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
            cmd.Parameters.AddWithValue("@Phone", person.Phone);
            cmd.Parameters.AddWithValue("@Address", person.Address);
            cmd.Parameters.AddWithValue("@PersonID", person.PersonID);
            cmd.Parameters.AddWithValue("@Gender", person.Gender);
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

        public async  Task<bool> DeletePersonAsync(int PersonID)
        {
            using SqlConnection connection =
            new SqlConnection(_ConnString);
            using SqlCommand cmd = new SqlCommand("SP_DeletePerson", connection);

            int RowsAffected = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", PersonID);

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
