using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.PersonDTOs;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class PersonService : IPersonService
    {

        private readonly IPersonRepo _repo;

        public PersonService(IPersonRepo repo)
        {
            _repo = repo;
        }
        public async Task<int?> AddNewPersonAsync(CreatePersonDTO person)
        {
            if (!person.IsValid())
                return null;

            var PersonEntity = Mapping.PersonMap.ToEntity(person);

            if (PersonEntity == null)
                return null;

            return await _repo.AddNewPersonAsync(PersonEntity);
        }                                          
        public async Task<bool> UpdatePersonAsync(UpdatePersonDTO person)
        {
            if (person == null)
                return false;

            var PersonEntity = Mapping.PersonMap.ToEntity(person);

            return await _repo.UpdatePersonAsync(PersonEntity);
        }
        public async Task<PersonResponseDTO?> GetPersonByIDAsync(int id)
        {
            var PersonEntity = await _repo.GetPersonByIDAsync(id);

            if (PersonEntity == null)
                return null;

            return PersonMap.ToPersonDTO(PersonEntity);            
        }
        public async Task<List<PersonResponseDTO>> GetAllPeopleAsync()
        {
            var ListOfPeople = await _repo.GetAllPeopleAsync();

            return PersonMap.ToDTOList(ListOfPeople);               
        }
        public async Task<bool> DeletePersonByIDAsync(int id)
        {
            if (id < 1)
                return false;

            bool isDeleted =  await _repo.DeletePersonAsync(id);

            return isDeleted;
        }
       
    }
}
