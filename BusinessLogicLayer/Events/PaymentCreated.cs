using MediatR;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Events
{
    public class PaymentCreated
    {
        public record PaymentCreatedEvent
            (
                int PaymentID,
                int OrderID,
                decimal PaidAmoun,
                DateTime PaidAt
            ) : INotification;
    }
}