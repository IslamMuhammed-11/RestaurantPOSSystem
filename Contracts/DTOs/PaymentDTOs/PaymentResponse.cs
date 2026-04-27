using System;

namespace Contracts.DTOs.PaymentDTOs
{
    public class PaymentResponse
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public int PaymentMethodID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
