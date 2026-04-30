using BusinessLogicLayer.Interfaces;
using Contracts.Exceptions;
using Contracts.Enums;
using Contracts.DTOs.PaymentDTOs;
using DataAccessLayer.Entites;
using DataAccessLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using BusinessLogicLayer.Events;

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

        public async Task<int?> CreateNewPaymentAsync(int orderId, CreatePaymentRequest payment)
        {
            if (payment == null || !payment.IsValid())
                throw new BusinessException("Invalid payment data.", 90000, Enums.ActionResult.InvalidData);
            // Ensure order exists
            var order = await _orderRepo.GetOrderByIDAsync(orderId);
            if (order == null)
                throw new BusinessException("Order not found.", 90003, Enums.ActionResult.NotFound);

            // Ensure order isn't cancelled
            if (order.OrderStatus == OrderEntity.enOrderStatus.Cancelled)
                throw new BusinessException("Cannot pay for a cancelled order.", 90004, Enums.ActionResult.InvalidData);

            // Check if a payment already exists for this order
            if (await _paymentRepo.IsPaid(orderId))
                throw new BusinessException("Payment for this order already exists.", 90005, Enums.ActionResult.Conflict);

            // Check paid amount equals order total
            if (payment.PaymentAmount != order.TotalPrice)
                throw new BusinessException("Paid amount does not match order total.", 90006, Enums.ActionResult.InvalidData);

            if (await _paymentMethodService.GetMethodByIdAsync(payment.PaymentMethodID) == null)
                throw new BusinessException($"Invalid payment method Id {payment.PaymentMethodID}", 90007, Enums.ActionResult.NotFound);

            // Map DTO to entity
            var entity = PaymentMap.ToEntity(payment, orderId);

            int? ID = await _paymentRepo.CreateNewPaymentAsync(entity);

            if (!ID.HasValue)
                return null;

            await _mediator.Publish(new PaymentCreated.PaymentCreatedEvent
                (entity.PaymentID, entity.OrderID, entity.PaidAmount, DateTime.Now));

            return ID;
        }

        public async Task<List<PaymentResponse>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepo.GetAllPaymentsAsync();
            return PaymentMap.ToReadDTOList(payments);
        }

        public async Task<PaymentResponse?> GetPaymentByOrderIdAsync(int orderId)
        {
            if (orderId < 0)
                throw new BusinessException("Order Id must be non negative number", 80000, Enums.ActionResult.InvalidData);

            var order = await _orderRepo.GetOrderByIDAsync(orderId);

            if (order == null)
                throw new BusinessException($"Order with this Id was not found {orderId}", 80000, Enums.ActionResult.NotFound);

            var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);

            if (payment == null)
                throw new BusinessException("Payment not found", 80000, Enums.ActionResult.NotFound);

            return PaymentMap.ToReadDTO(payment);
        }

        public async Task<PaymentResponse?> GetPaymentByPaymentIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("Invalid payment id.", 90007, Enums.ActionResult.InvalidData);

            var payment = await _paymentRepo.GetPaymentByPaymentIdAsync(id);
            if (payment == null)
                throw new BusinessException("Payment not found.", 90008, Enums.ActionResult.NotFound);

            return PaymentMap.ToReadDTO(payment);
        }
    }
}