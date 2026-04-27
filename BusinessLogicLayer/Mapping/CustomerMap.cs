using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.CustomerDTOs;
using DataAccessLayer.Entites;
namespace BusinessLogicLayer.Mapping
{
    public class CustomerMap
    {
        public static CustomerResponse ToReadDTO(CustomerEntity entity)
        {
            return new CustomerResponse
            {
                CustomerID = entity.CustomerID,
                Name = entity.Name,
                Phone = entity.Phone
            };
        }

        public static CustomerEntity ToEntity(CreateCustomerRequest dto)
        {
            return new CustomerEntity
            {
                Name = dto.Name,
                Phone = dto.Phone
            };
        }

        public static bool ToEntity(UpdateCustomerRequest dto, CustomerEntity customer)
        {
            if (dto == null)
                return false;
            if (!string.IsNullOrEmpty(dto.Name))
                customer.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Phone))
                customer.Phone = dto.Phone;
            return true;
        }

        public static List<CustomerResponse> ToReadDTOList(List<CustomerEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<CustomerResponse>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }

        
    }
}
