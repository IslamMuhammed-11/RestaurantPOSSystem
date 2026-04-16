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
        public static CreatePersonDTO ToCreateDTO(PersonEntity entity)
        {
            return new CreatePersonDTO
            {
                FirstName = entity.FirstName,
                SecondName = entity.SecondName,
                ThirdName = entity.ThirdName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                Phone = entity.Phone,
                Address = entity.Address,
            };            
        }
        public static PersonEntity ToEntity(CreatePersonDTO personDTO)
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
        public static UpdatePersonDTO ToUpdateDTO(PersonEntity entity)
        {
            return new UpdatePersonDTO
            {
                PersonID = entity.PersonID,
                FirstName = entity.FirstName,
                SecondName = entity.SecondName,
                ThirdName = entity.ThirdName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                Phone = entity.Phone,
                Address = entity.Address,
            };
        }
        public static PersonEntity ToEntity(UpdatePersonDTO personDTO)
        {
            return new PersonEntity
            {
                PersonID = personDTO.PersonID,
                FirstName = personDTO.FirstName,
                SecondName = personDTO.SecondName,
                ThirdName = personDTO.ThirdName,
                DateOfBirth = personDTO.DateOfBirth,
                Gender = personDTO.Gender,
                Phone = personDTO.Phone,
                Address = personDTO.Address
            };
        }
        public static PersonResponseDTO ToPersonDTO(PersonEntity entity)
        {
            return new PersonResponseDTO
            {
                PersonID = entity.PersonID,
                FirstName = entity.FirstName,
                SecondName = entity.SecondName,
                ThirdName = entity.ThirdName,
                DateOfBirth = entity.DateOfBirth,
                Gender = entity.Gender,
                Phone = entity.Phone,
                Address = entity.Address,
            };
        }
        public static PersonEntity ToEntity(PersonResponseDTO personDTO)
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
        public static List<PersonResponseDTO> ToDTOList(List<PersonEntity> entityList)
        {
            List<PersonResponseDTO> list = new List<PersonResponseDTO>();
            foreach (PersonEntity entity in entityList)
            {
                list.Add(ToPersonDTO(entity));
            }

            return list;
        }
    }
}
