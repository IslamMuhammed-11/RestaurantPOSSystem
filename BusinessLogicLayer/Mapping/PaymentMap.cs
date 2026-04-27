using Contracts.DTOs.PaymentDTOs;
using DataAccessLayer.Entites;
using System;

namespace BusinessLogicLayer.Mapping
{
    public static class PaymentMap
    {
        public static PaymentEntity ToEntity(CreatePaymentRequest dto, int orderId)
        {
            if (dto == null) return null!;
            return new PaymentEntity
            {
                OrderID = orderId,
                PaymentMethodID = dto.PaymentMethodID,
                PaymentDate = DateTime.UtcNow,
                PaidAmount = dto.PaymentAmount
            };
        }

        public static PaymentResponse ToReadDTO(PaymentEntity entity)
        {
            if (entity == null) return null!;
            return new PaymentResponse
            {
                PaymentID = entity.PaymentID,
                OrderID = entity.OrderID,
                PaymentMethodID = entity.PaymentMethodID,
                PaymentDate = entity.PaymentDate,
                PaymentAmount = entity.PaidAmount
            };
        }

        public static System.Collections.Generic.List<PaymentResponse> ToReadDTOList(System.Collections.Generic.List<PaymentEntity> entities)
        {
            if (entities == null || entities.Count == 0) return new System.Collections.Generic.List<PaymentResponse>();
            var list = new System.Collections.Generic.List<PaymentResponse>();
            foreach (var e in entities)
                list.Add(ToReadDTO(e));
            return list;
        }
    }
}