using BusinessLogicLayer.Events;
using BusinessLogicLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Handlers
{
    public class UpdateDailySalesHandler : INotificationHandler<PaymentCreated.PaymentCreatedEvent>
    {
        private readonly IDailySalesService _dailySalesService;
        private readonly IProductSalesService _productsSalesService;
        private readonly ILogger<UpdateDailySalesHandler> _logger;

        public UpdateDailySalesHandler(IDailySalesService dailySalesService, IProductSalesService productsSalesService, ILogger<UpdateDailySalesHandler> logger)
        {
            _dailySalesService = dailySalesService;
            _productsSalesService = productsSalesService;
            _logger = logger;
        }

        public async Task Handle(PaymentCreated.PaymentCreatedEvent notification, CancellationToken cancellationToken)
        {
            var logSale = _dailySalesService.LogDailySalesAsync(notification.PaidAmoun);
            var logProduct = _productsSalesService.LogProductSalesAsync(notification.OrderID);

            await Task.WhenAll(logSale, logProduct);

            if (!logSale.Result)
                _logger.LogError("Could Not Log Sale at Payment ID = {payment}", notification.PaymentID);

            if (!logProduct.Result)
                _logger.LogError("Could Not Log Product at Payment ID = {payment}", notification.PaymentID);
        }
    }
}