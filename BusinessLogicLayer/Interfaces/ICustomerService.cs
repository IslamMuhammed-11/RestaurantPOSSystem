using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.DTOs.CustomerDTOs;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<CustomerResponse>> AddNewCustomerAsync(CreateCustomerRequest customer);

        Task<Result<CustomerResponse>> UpdateCustomerAsync(int ID, UpdateCustomerRequest customer);

        Task<Result<CustomerResponse>> GetCustomerByIDAsync(int id);

        Task<Result<List<CustomerResponse>>> GetAllCustomersAsync();

        Task<Result<bool>> DeleteCustomerByIDAsync(int id);
    }
}