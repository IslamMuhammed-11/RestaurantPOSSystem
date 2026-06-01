using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.PaymentMethodDTOs;
using Contracts.Enums;
using Contracts.Exceptions;
using DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepo _repo;

        public PaymentMethodService(IPaymentMethodRepo repo)
        {
            _repo = repo;
        }

        public async Task<Result<PaymentMethodResponse>> AddNewMethodAsync(CreatePaymentMethodRequest dto)
        {
            if (dto == null || !dto.IsValid())
                return Result<PaymentMethodResponse>.Failure(new Error("Invalid payment method data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var entity = PaymentMethodMap.ToEntity(dto);
            int? id = await _repo.AddNewMethod(entity);

            if (!id.HasValue)
                return Result<PaymentMethodResponse>.Failure(new Error("Failed to add payment method.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new PaymentMethodResponse
            {
                MethodID = id.Value,
                PaymentMethod = dto.PaymentMethod
            };

            return Result<PaymentMethodResponse>.Success(response);
        }

        public async Task<Result<List<PaymentMethodResponse>>> GetAllMethodsAsync()
        {
            var entities = await _repo.GetAllMethods();
            return Result<List<PaymentMethodResponse>>.Success(PaymentMethodMap.ToReadDTOList(entities));
        }

        public async Task<Result<PaymentMethodResponse?>> GetMethodByIdAsync(int id)
        {
            if (id <= 0)
                return Result<PaymentMethodResponse?>.Failure(new Error("Invalid payment method id.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _repo.GetMethodByIdAsync(id);
            if (existing == null)
                return Result<PaymentMethodResponse?>.Failure(new Error("Payment method not found.", ErrorCodes.enErrorCodes.NOT_FOUND));
            return Result<PaymentMethodResponse?>.Success(PaymentMethodMap.ToReadDTO(existing));
        }

        public async Task<Result<PaymentMethodResponse>> UpdateMethodAsync(int id, UpdatePaymentMethodRequest dto)
        {
            if (id <= 0 || dto == null || !dto.IsValid())
                return Result<PaymentMethodResponse>.Failure(new Error("Invalid payment method data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var existing = await _repo.GetMethodByIdAsync(id);
            if (existing == null)
                return Result<PaymentMethodResponse>.Failure(new Error("Payment method not found.", ErrorCodes.enErrorCodes.NOT_FOUND));
            bool ok = PaymentMethodMap.ToEntity(dto, existing);
            if (!ok)
                return Result<PaymentMethodResponse>.Failure(new Error("Invalid payment method data.", ErrorCodes.enErrorCodes.INVALID_DATA));

            bool updated = await _repo.UpdateMethod(existing);

            if (!updated)
                return Result<PaymentMethodResponse>.Failure(new Error("Failed to update payment method.", ErrorCodes.enErrorCodes.DB_ERROR));

            var response = new PaymentMethodResponse
            {
                MethodID = existing.MethodID,
                PaymentMethod = existing.PaymentMethod
            };

            return Result<PaymentMethodResponse>.Success(response);
        }

        public async Task<Result<bool>> DeleteMethodAsync(int id)
        {
            if (id <= 0)
                return Result<bool>.Failure(new Error("Invalid payment method id.", ErrorCodes.enErrorCodes.INVALID_DATA));
            var existing = await _repo.GetMethodByIdAsync(id);
            if (existing == null)
                return Result<bool>.Failure(new Error("Payment method not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            bool deleted = await _repo.DeleteMethod(id);

            return Result<bool>.Success(deleted);
        }
    }
}