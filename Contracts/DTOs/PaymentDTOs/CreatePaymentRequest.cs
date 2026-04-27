using System;

namespace Contracts.DTOs.PaymentDTOs
{
    public class CreatePaymentRequest
    {
        public int PaymentMethodID { get; set; }
        public decimal PaymentAmount { get; set; }

        public bool IsValid() => PaymentMethodID > 0 && PaymentAmount > 0m;
    }
}