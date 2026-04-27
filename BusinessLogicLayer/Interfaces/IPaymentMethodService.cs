using Contracts.DTOs.PaymentMethodDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPaymentMethodService
    {

        Task<int?> AddNewMethodAsync(CreatePaymentMethodRequest dto);

        Task<List<PaymentMethodResponse>> GetAllMethodsAsync();

        Task<PaymentMethodResponse?> GetMethodByIdAsync(int id);

        Task<bool> UpdateMethodAsync(int id, UpdatePaymentMethodRequest dto);

        Task<bool> DeleteMethodAsync(int id);
    }
}
