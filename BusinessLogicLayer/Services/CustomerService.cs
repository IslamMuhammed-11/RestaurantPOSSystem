using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.CustomerDTOs;
using Contracts.Enums;
namespace BusinessLogicLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomerService(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<int?> AddNewCustomerAsync(CreateCustomerRequest customer)
        {

            if (customer == null || string.IsNullOrEmpty(customer.Name))
                return null;

            var customerEntity = Mapping.CustomerMap.ToEntity(customer);

            return await _customerRepo.AddNewCustomerAsync(customerEntity);
        }

        public async Task<List<CustomerResponse>> GetAllCustomersAsync()
        {
            var customers = await _customerRepo.GetAllCustomersAsync();
            return Mapping.CustomerMap.ToReadDTOList(customers);
        }

        public async Task<CustomerResponse?> GetCustomerByIDAsync(int id)
        {
            var customer = await _customerRepo.GetCustomerByIDAsync(id);
            if (customer == null)
                return null;
            return Mapping.CustomerMap.ToReadDTO(customer);
        }

        public async Task<Enums.ActionResult> UpdateCustomerAsync(int ID, UpdateCustomerRequest customer)
        {
            var existingCustomer = await _customerRepo.GetCustomerByIDAsync(ID);
            if (existingCustomer == null)
                return Enums.ActionResult.NotFound;
            bool isUpdated = Mapping.CustomerMap.ToEntity(customer, existingCustomer);
            if (!isUpdated)
                return Enums.ActionResult.InvalidData;
            bool updateResult = await _customerRepo.UpdateCustomerAsync(existingCustomer);
            return updateResult ? Enums.ActionResult.Success : Enums.ActionResult.DBError;
        }

        public async Task<Enums.ActionResult> DeleteCustomerByIDAsync(int id)
        {
            var existingCustomer = await _customerRepo.GetCustomerByIDAsync(id);
            if (existingCustomer == null)
                return Enums.ActionResult.NotFound;
            bool deleteResult = await _customerRepo.DeleteCustomerAsync(id);

            return deleteResult ? Enums.ActionResult.Success : Enums.ActionResult.DBError;
        }
    }
}
