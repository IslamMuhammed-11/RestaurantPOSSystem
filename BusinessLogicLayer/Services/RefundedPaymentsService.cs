using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.RefundedPaymentsDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class RefundedPaymentsService : IRefundedPaymentsService
    {
        private readonly IRefundedPaymentsRepo _refundedPaymentsRepo;
        private readonly IPaymentRepo _paymentRepo;

        public RefundedPaymentsService(IRefundedPaymentsRepo refundedPaymentsRepo, IPaymentRepo paymentRepo)
        {
            _refundedPaymentsRepo = refundedPaymentsRepo;
            _paymentRepo = paymentRepo;
        }

        public async Task<int?> RefundPaymentAsync(CreateRefundPaymentRequest refund)
        {
            if (refund == null || !refund.IsValid())
                throw new BusinessException("Invalid refund data.", 91000, Enums.ActionResult.InvalidData);

            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(refund.PaymentID);
            if (payment == null)
                throw new BusinessException("Payment not found.", 91001, Enums.ActionResult.NotFound);

            if (refund.Amount > payment.PaidAmount)
                throw new BusinessException("Refund amount cannot exceed the paid amount.", 91002, Enums.ActionResult.InvalidData);

            var entity = RefundedPaymentsMap.ToEntity(refund);
            int? refundId = await _refundedPaymentsRepo.RefundPaymentAsync(entity);

            if (!refundId.HasValue)
                throw new BusinessException("Failed to create refund.", 91003, Enums.ActionResult.DBError);

            return refundId;
        }

        public async Task<List<RefundedPaymentResponse>> GetAllRefundedPaymentsAsync()
        {
            var refunds = await _refundedPaymentsRepo.GetAllRefundedPaymentsAsync();
            return RefundedPaymentsMap.ToReadDTOList(refunds);
        }

        public async Task<RefundedPaymentResponse?> GetRefundedPaymentByIDAsync(int refundId)
        {
            if (refundId <= 0)
                throw new BusinessException("Invalid refund ID.", 91004, Enums.ActionResult.InvalidData);

            var refund = await _refundedPaymentsRepo.GetRefundedPaymentByIDAsync(refundId);
            if (refund == null)
                throw new BusinessException("Refund not found.", 91005, Enums.ActionResult.NotFound);

            return RefundedPaymentsMap.ToReadDTO(refund);
        }
    }
}
