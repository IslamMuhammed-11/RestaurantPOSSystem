namespace Contracts.DTOs.RefundedPaymentsDTOs
{
    public class CreateRefundPaymentRequest
    {
        public int PaymentID { get; set; }

        public string Reason { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public bool IsValid()
        {
            return PaymentID > 0
                && !string.IsNullOrWhiteSpace(Reason)
                && Amount > 0m;
        }
    }
}
