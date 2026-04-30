using BusinessLogicLayer.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Handlers
{
    public class UpdateDailySalesHandler : INotificationHandler<PaymentCreated.PaymentCreatedEvent>
    {
        public Task Handle(PaymentCreated.PaymentCreatedEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //updating the daily sales table
        }
    }
}