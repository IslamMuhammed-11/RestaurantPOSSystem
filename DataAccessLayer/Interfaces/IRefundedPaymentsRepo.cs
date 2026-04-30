using DataAccessLayer.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IRefundedPaymentsRepo
    {
        Task<int?> RefundPaymentAsync(RefundedPaymentsEntity refund);

        Task<List<RefundedPaymentsEntity>> GetAllRefundedPaymentsAsync();

        Task<RefundedPaymentsEntity?> GetRefundedPaymentByIDAsync(int refundId);
    }
}
