using Contracts.DTOs.PaymentDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<List<PaymentResponse>>> GetAllPaymentsAsync();

        Task<Result<PaymentResponse>> GetPaymentByPaymentIdAsync(int id);

        Task<Result<PaymentResponse>> GetPaymentByOrderIdAsync(int orderId);

        Task<Result<PaymentResponse>> CreateNewPaymentAsync(int orderId, CreatePaymentRequest payment);
    }
}