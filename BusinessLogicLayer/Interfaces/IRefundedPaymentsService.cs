using Contracts.DTOs.RefundedPaymentsDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IRefundedPaymentsService
    {
        Task<int?> RefundPaymentAsync(CreateRefundPaymentRequest refund);

        Task<List<RefundedPaymentResponse>> GetAllRefundedPaymentsAsync();

        Task<RefundedPaymentResponse?> GetRefundedPaymentByIDAsync(int refundId);
    }
}
