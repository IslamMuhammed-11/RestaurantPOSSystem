using Contracts.DTOs.RefundedPaymentsDTOs;
using DataAccessLayer.Entites;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Mapping
{
    public static class RefundedPaymentsMap
    {
        public static RefundedPaymentsEntity ToEntity(CreateRefundPaymentRequest dto)
        {
            if (dto == null)
                return null!;

            return new RefundedPaymentsEntity
            {
                PaymentID = dto.PaymentID,
                Reason = dto.Reason?.Trim() ?? string.Empty,
                Amount = dto.Amount
            };
        }

        public static RefundedPaymentResponse ToReadDTO(RefundedPaymentsEntity entity)
        {
            if (entity == null)
                return null!;

            return new RefundedPaymentResponse
            {
                RefundID = entity.RefundID,
                PaymentID = entity.PaymentID,
                Reason = entity.Reason,
                Amount = entity.Amount,
                RefundedAt = entity.RefundedAt
            };
        }

        public static List<RefundedPaymentResponse> ToReadDTOList(List<RefundedPaymentsEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<RefundedPaymentResponse>();

            return entities.Select(ToReadDTO).ToList();
        }
    }
}
