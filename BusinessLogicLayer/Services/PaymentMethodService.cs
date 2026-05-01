using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.PaymentMethodDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepo _repo;

        public PaymentMethodService(IPaymentMethodRepo repo)
        {
            _repo = repo;
        }

        public async Task<int?> AddNewMethodAsync(CreatePaymentMethodRequest dto)
        {
            if (dto == null || !dto.IsValid())
                throw new BusinessException("Invalid payment method data.", 92000, ActionResultEnum.ActionResult.InvalidData);

            var entity = PaymentMethodMap.ToEntity(dto);
            int? id = await _repo.AddNewMethod(entity);
            if (!id.HasValue)
                throw new BusinessException("Failed to create payment method.", 92002, ActionResultEnum.ActionResult.DBError);

            dto.SetID(id.Value);
            return id;
        }

        public async Task<List<PaymentMethodResponse>> GetAllMethodsAsync()
        {
            var entities = await _repo.GetAllMethods();
            return PaymentMethodMap.ToReadDTOList(entities);
        }

        public async Task<PaymentMethodResponse?> GetMethodByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("Invalid payment method id.", 92000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _repo.GetMethodByIdAsync(id);
            if (existing == null)
                throw new BusinessException("Payment method not found.", 92001, ActionResultEnum.ActionResult.NotFound);

            return PaymentMethodMap.ToReadDTO(existing);
        }

        public async Task<bool> UpdateMethodAsync(int id, UpdatePaymentMethodRequest dto)
        {
            if (id <= 0 || dto == null || !dto.IsValid())
                throw new BusinessException("Invalid payment method data.", 92000, ActionResultEnum.ActionResult.InvalidData);

            var existing = await _repo.GetMethodByIdAsync(id);
            if (existing == null)
                throw new BusinessException("Payment method not found.", 92001, ActionResultEnum.ActionResult.NotFound);

            bool ok = PaymentMethodMap.ToEntity(dto, existing);
            if (!ok)
                throw new BusinessException("Invalid payment method data.", 92000, ActionResultEnum.ActionResult.InvalidData);

            bool updated = await _repo.UpdateMethod(existing);
            if (!updated)
                throw new BusinessException("Failed to update payment method.", 92003, ActionResultEnum.ActionResult.DBError);

            return true;
        }

        public async Task<bool> DeleteMethodAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("Invalid payment method id.", 92000, ActionResultEnum.ActionResult.InvalidData);
            var existing = await _repo.GetMethodByIdAsync(id);
            if (existing == null)
                throw new BusinessException("Payment method not found.", 92001, ActionResultEnum.ActionResult.NotFound);

            bool deleted = await _repo.DeleteMethod(id);
            if (!deleted)
                throw new BusinessException("Failed to delete payment method.", 92003, ActionResultEnum.ActionResult.DBError);

            return true;
        }
    }
}
