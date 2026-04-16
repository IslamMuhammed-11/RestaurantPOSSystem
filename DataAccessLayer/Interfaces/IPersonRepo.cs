using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPersonRepo
    {
        Task<PersonEntity?> GetPersonByIDAsync(int id);
        Task<List<PersonEntity>> GetAllPeopleAsync();
        Task<int?> AddNewPersonAsync(PersonEntity person);
        Task<bool> UpdatePersonAsync(PersonEntity person);
        Task<bool> DeletePersonAsync(int PersonID);
    }
}
