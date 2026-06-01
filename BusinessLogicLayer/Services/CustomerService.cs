using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.CustomerDTOs;
using Contracts.Enums;
using Contracts.ErrorHandling;
using DataAccessLayer.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomerService(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<Result<CustomerResponse>> AddNewCustomerAsync(CreateCustomerRequest customer)
        {
            if (customer == null || string.IsNullOrEmpty(customer.Name))
                return Result<CustomerResponse>.Failure(new Error("Invalid customer data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var customerEntity = Mapping.CustomerMap.ToEntity(customer);

            int? id = await _customerRepo.AddNewCustomerAsync(customerEntity);

            if (!id.HasValue)
                return Result<CustomerResponse>.Failure(new Error("Failed to add new customer.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new CustomerResponse
            {
                CustomerID = id.Value,
                Name = customer.Name,
                Phone = customer.Phone
            };

            return Result<CustomerResponse>.Success(response);
        }

        public async Task<Result<List<CustomerResponse>>> GetAllCustomersAsync()
        {
            var customers = await _customerRepo.GetAllCustomersAsync();
            return Result<List<CustomerResponse>>.Success(Mapping.CustomerMap.ToReadDTOList(customers));
        }

        public async Task<Result<CustomerResponse>> GetCustomerByIDAsync(int id)
        {
            if (id < 0)
                return Result<CustomerResponse>.Failure(new Error("ID Cannot be negative.", ErrorCodes.enErrorCodes.INVALID_ID));

            var customer = await _customerRepo.GetCustomerByIDAsync(id);
            if (customer == null)
                return Result<CustomerResponse>.Failure(new Error("Customer Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<CustomerResponse>.Success(Mapping.CustomerMap.ToReadDTO(customer));
        }

        public async Task<Result<CustomerResponse>> UpdateCustomerAsync(int ID, UpdateCustomerRequest customer)
        {
            var existingCustomer = await _customerRepo.GetCustomerByIDAsync(ID);
            if (existingCustomer == null)
                return Result<CustomerResponse>.Failure(new Error("Customer Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));
            bool isUpdated = Mapping.CustomerMap.ToEntity(customer, existingCustomer);
            if (!isUpdated)
                return Result<CustomerResponse>.Failure(new Error("Invalid customer data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            bool updateResult = await _customerRepo.UpdateCustomerAsync(existingCustomer);

            if (!updateResult)
                return Result<CustomerResponse>.Failure(new Error("Failed to update customer.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<CustomerResponse>.Success(Mapping.CustomerMap.ToReadDTO(existingCustomer));
        }

        public async Task<Result<bool>> DeleteCustomerByIDAsync(int id)
        {
            if (id < 0)
                return Result<bool>.Failure(new Error("ID Cannot be negative.", ErrorCodes.enErrorCodes.INVALID_ID));

            var existingCustomer = await _customerRepo.GetCustomerByIDAsync(id);

            if (existingCustomer == null)
                return Result<bool>.Failure(new Error("Customer Not Found", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool deleteResult = await _customerRepo.DeleteCustomerAsync(id);

            if (!deleteResult)
                return Result<bool>.Failure(new Error("Failed to delete customer.", ErrorCodes.enErrorCodes.DB_ERROR));

            return Result<bool>.Success(deleteResult);
        }
    }
}