using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.CustomerDTOs;
namespace BusinessLogicLayer.Interfaces
{
    public interface ICustomerService
    {
            Task<int?> AddNewCustomerAsync(CreateCustomerDTO customer);
            Task<Enums.ActionResult> UpdateCustomerAsync(int ID, UpdateCustomerDTO customer);
            Task<ReadCustomerDTO?> GetCustomerByIDAsync(int id);
            Task<List<ReadCustomerDTO>> GetAllCustomersAsync();
            Task<Enums.ActionResult> DeleteCustomerByIDAsync(int id);
    }
}
