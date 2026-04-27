using Contracts.DTOs.PaymentMethodDTOs;
using DataAccessLayer.Entites;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Mapping
{
    public static class PaymentMethodMap
    {
        public static PaymentMethodEntity ToEntity(CreatePaymentMethodRequest dto)
        {
            if (dto == null) return null!;
            return new PaymentMethodEntity
            {
                PaymentMethod = dto.PaymentMethod
            };
        }

        public static PaymentMethodResponse ToReadDTO(PaymentMethodEntity entity)
        {
            if (entity == null) return null!;
            return new PaymentMethodResponse
            {
                MethodID = entity.MethodID,
                PaymentMethod = entity.PaymentMethod
            };
        }

        public static List<PaymentMethodResponse> ToReadDTOList(List<PaymentMethodEntity> entities)
        {
            if (entities == null || entities.Count == 0) return new List<PaymentMethodResponse>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }

        public static bool ToEntity(UpdatePaymentMethodRequest dto, PaymentMethodEntity existing)
        {
            if (dto == null || existing == null) return false;
            if (!string.IsNullOrEmpty(dto.PaymentMethod))
                existing.PaymentMethod = dto.PaymentMethod;
            return true;
        }
    }
}
