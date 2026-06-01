using BusinessLogicLayer.Events;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.PaymentDTOs;
using Contracts.Enums;
using Contracts.ErrorHandling;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using MediatR;

namespace BusinessLogicLayer.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IMediator _mediator;

        public PaymentService(IPaymentRepo paymentRepo, IOrderRepo orderRepo,
            IPaymentMethodService paymentMethodService, IMediator mediator)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _paymentMethodService = paymentMethodService;
            _mediator = mediator;
        }

        public async Task<Result<PaymentResponse>> CreateNewPaymentAsync(int orderId, CreatePaymentRequest payment)
        {
            if (payment == null || !payment.IsValid())
                return Result<PaymentResponse>.Failure(new Error("Invalid payment data.", ErrorCodes.enErrorCodes.INVALID_DATA));
            // Ensure order exists
            var order = await _orderRepo.GetOrderByIDAsync(orderId);
            if (order == null)
                return Result<PaymentResponse>.Failure(new Error("Order not found.", ErrorCodes.enErrorCodes.NOT_FOUND));
            // Ensure order isn't cancelled
            if (order.OrderStatus == OrderEntity.enOrderStatus.Cancelled)
                return Result<PaymentResponse>.Failure(new Error("Cannot pay for a cancelled order.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            // Check if a payment already exists for this order
            if (await _paymentRepo.IsPaid(orderId))
                return Result<PaymentResponse>.Failure(new Error("Payment for this order already exists.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));
            // Check paid amount equals order total
            if (payment.PaymentAmount != order.TotalPrice)
                return Result<PaymentResponse>.Failure(new Error("Paid amount does not match order total.", ErrorCodes.enErrorCodes.BUSINESS_RULE_VIOLATION));

            var paymentMethodResult = await _paymentMethodService.GetMethodByIdAsync(payment.PaymentMethodID);
            if (!paymentMethodResult.IsSuccess)
                return Result<PaymentResponse>.Failure(new Error($"Invalid payment method Id {payment.PaymentMethodID}", ErrorCodes.enErrorCodes.NOT_FOUND));
            // Map DTO to entity
            var entity = PaymentMap.ToEntity(payment, orderId);

            int? ID = await _paymentRepo.CreateNewPaymentAsync(entity);

            if (!ID.HasValue)
                return Result<PaymentResponse>.Failure(new Error("Failed to create payment.", ErrorCodes.enErrorCodes.DB_ERROR));

            await _mediator.Publish(new PaymentCreated.PaymentCreatedEvent
              (ID.Value, entity.OrderID, entity.PaidAmount));

            var response = new PaymentResponse
            {
                PaymentID = ID.Value,
                OrderID = entity.OrderID,
                PaymentAmount = entity.PaidAmount,
                PaymentMethodID = entity.PaymentMethodID,
                PaymentDate = entity.PaymentDate
            };

            return Result<PaymentResponse>.Success(response);
        }

        public async Task<Result<List<PaymentResponse>>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepo.GetAllPaymentsAsync();
            return Result<List<PaymentResponse>>.Success(PaymentMap.ToReadDTOList(payments));
        }

        public async Task<Result<PaymentResponse>> GetPaymentByOrderIdAsync(int orderId)
        {
            if (orderId < 0)
                return Result<PaymentResponse>.Failure(new Error("Order Id must be non negative number", ErrorCodes.enErrorCodes.INVALID_DATA));

            var order = await _orderRepo.GetOrderByIDAsync(orderId);

            if (order == null)
                return Result<PaymentResponse>.Failure(new Error($"Order with this Id was not found {orderId}", ErrorCodes.enErrorCodes.NOT_FOUND));

            var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);

            if (payment == null)
                return Result<PaymentResponse>.Failure(new Error("Payment not found", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<PaymentResponse>.Success(PaymentMap.ToReadDTO(payment));
        }

        public async Task<Result<PaymentResponse>> GetPaymentByPaymentIdAsync(int id)
        {
            if (id <= 0)
                return Result<PaymentResponse>.Failure(new Error("Invalid payment id.", ErrorCodes.enErrorCodes.INVALID_DATA));

            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(id);
            if (payment == null)
                return Result<PaymentResponse>.Failure(new Error("Payment not found.", ErrorCodes.enErrorCodes.NOT_FOUND));

            return Result<PaymentResponse>.Success(PaymentMap.ToReadDTO(payment));
        }
    }
}