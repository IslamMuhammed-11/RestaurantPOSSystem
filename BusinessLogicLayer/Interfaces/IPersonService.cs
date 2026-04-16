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
        Task<int?> AddNewPersonAsync(CreatePersonDTO person);
        Task<bool> UpdatePersonAsync(UpdatePersonDTO person);
        Task<PersonResponseDTO?> GetPersonByIDAsync(int id);
        Task<List<PersonResponseDTO>> GetAllPeopleAsync();
        Task<bool> DeletePersonByIDAsync(int id);
    }
}
