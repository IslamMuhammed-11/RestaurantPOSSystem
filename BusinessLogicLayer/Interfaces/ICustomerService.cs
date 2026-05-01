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
            Task<int?> AddNewCustomerAsync(CreateCustomerRequest customer);
            Task<ActionResultEnum.ActionResult> UpdateCustomerAsync(int ID, UpdateCustomerRequest customer);
            Task<CustomerResponse?> GetCustomerByIDAsync(int id);
            Task<List<CustomerResponse>> GetAllCustomersAsync();
            Task<ActionResultEnum.ActionResult> DeleteCustomerByIDAsync(int id);
    }
}
