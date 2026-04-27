namespace Contracts.DTOs.PaymentDTOs
{
    public class CreatePaymentCreatedResponse
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
