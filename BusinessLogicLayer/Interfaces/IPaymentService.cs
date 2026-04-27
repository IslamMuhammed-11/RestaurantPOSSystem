using Contracts.DTOs.PaymentDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentResponse>> GetAllPaymentsAsync();

        Task<PaymentResponse?> GetPaymentByPaymentIdAsync(int id);

        Task<PaymentResponse?> GetPaymentByOrderIdAsync(int orderId);

        Task<int?> CreateNewPaymentAsync(int orderId, CreatePaymentRequest payment);
    }
}