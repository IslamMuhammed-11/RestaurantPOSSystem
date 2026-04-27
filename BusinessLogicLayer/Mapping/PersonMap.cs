using Contracts.DTOs.PersonDTOs;
using Contracts.DTOs.UserDTOs;
using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Mapping
{
    public class PersonMap
    {
        public static CreatePersonRequest ToCreateDTO(PersonEntity entity)
        {
            return new CreatePersonRequest
            {
                FirstName = entity.FirstName,
                SecondName = entity.SecondName ?? entity.ThirdName ?? string.Empty,
                Phone = entity.Phone,
            };
        }

        public static PersonEntity ToEntity(CreatePersonRequest personDTO)
        {
            return new PersonEntity
            {
                FirstName = personDTO.FirstName,
                SecondName = personDTO.SecondName,
                ThirdName = personDTO.ThirdName,
                DateOfBirth = personDTO.DateOfBirth,
                Gender = personDTO.Gender,
                Phone = personDTO.Phone,
                Address = personDTO.Address
            };
        }

        //public static PersonEntity ToEntity(CreateUserDTO user)
        //{
        //    return new PersonEntity
        //    {
        //        FirstName = user.FirstName,
        //        SecondName = user.SecondName,
        //        ThirdName = user.ThirdName,
        //        DateOfBirth = user.DateOfBirth,
        //        Gender = user.Gender,
        //        Phone = user.Phone,
        //        Address = user.Address
        //    };
        //}
        public static UpdatePersonRequest ToUpdateDTO(PersonEntity entity)
        {
            return new UpdatePersonRequest
            {
                FirstName = entity.FirstName,
                LastName = entity.SecondName ?? entity.ThirdName ?? string.Empty,
                Phone = entity.Phone,
            };
        }

        public static PersonEntity ToEntity(UpdatePersonRequest personDTO, PersonEntity existingPerson)
        {
            return new PersonEntity
            {
                PersonID = existingPerson.PersonID,
                FirstName = personDTO.FirstName ?? existingPerson.FirstName,
                SecondName = personDTO.LastName ?? existingPerson.SecondName,
                ThirdName = existingPerson.ThirdName,
                DateOfBirth = existingPerson.DateOfBirth,
                Gender = existingPerson.Gender,
                Phone = personDTO.Phone ?? existingPerson.Phone,
                Address = existingPerson.Address
            };
        }

        public static PersonResponse ToPersonDTO(PersonEntity entity)
        {
            return new PersonResponse
            {
                PersonID = entity.PersonID,
                FirstName = entity.FirstName,
                LastName = entity.SecondName ?? entity.ThirdName ?? string.Empty,
                Phone = entity.Phone,
            };
        }

        public static PersonEntity ToEntity(PersonResponse personDTO)
        {
            return new PersonEntity
            {
                FirstName = personDTO.FirstName,
                SecondName = personDTO.LastName,
                ThirdName = string.Empty,
                DateOfBirth = DateTime.MinValue,
                Gender = false,
                Phone = personDTO.Phone,
                Address = string.Empty
            };
        }

        public static List<PersonResponse> ToDTOList(List<PersonEntity> entityList)
        {
            List<PersonResponse> list = new List<PersonResponse>();
            foreach (PersonEntity entity in entityList)
            {
                list.Add(ToPersonDTO(entity));
            }

            return list;
        }
    }
}