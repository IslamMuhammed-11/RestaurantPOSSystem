using System;

namespace DataAccessLayer.Entites
{
    public class RefundedPaymentsEntity
    {
        public int RefundID { get; set; }

        public int PaymentID { get; set; }

        public string Reason { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime RefundedAt { get; set; }
    }
}
