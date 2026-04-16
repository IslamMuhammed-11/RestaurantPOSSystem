using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ICustomerRepo
    {
        Task<CustomerEntity?> GetCustomerByIDAsync(int id);
        Task<List<CustomerEntity>> GetAllCustomersAsync();
        Task<int?> AddNewCustomerAsync(CustomerEntity customer);
        Task<bool> UpdateCustomerAsync(CustomerEntity customer);
        Task<bool> DeleteCustomerAsync(int CustomerID);
    }
}
