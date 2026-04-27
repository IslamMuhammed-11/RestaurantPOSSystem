using Contracts.DTOs.PersonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPersonService
    {
        Task<int?> AddNewPersonAsync(CreatePersonRequest person);

        Task<bool> UpdatePersonAsync(int id, UpdatePersonRequest person);

        Task<PersonResponse?> GetPersonByIDAsync(int id);

        Task<List<PersonResponse>> GetAllPeopleAsync();

        Task<bool> DeletePersonByIDAsync(int id);
    }
}