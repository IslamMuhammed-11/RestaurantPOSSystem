using Contracts.DTOs.PaymentMethodDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPaymentMethodService
    {
        Task<Result<PaymentMethodResponse>> AddNewMethodAsync(CreatePaymentMethodRequest dto);

        Task<Result<List<PaymentMethodResponse>>> GetAllMethodsAsync();

        Task<Result<PaymentMethodResponse?>> GetMethodByIdAsync(int id);

        Task<Result<PaymentMethodResponse>> UpdateMethodAsync(int id, UpdatePaymentMethodRequest dto);

        Task<Result<bool>> DeleteMethodAsync(int id);
    }
}