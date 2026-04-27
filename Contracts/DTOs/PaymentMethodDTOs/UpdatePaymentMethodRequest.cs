namespace Contracts.DTOs.PaymentMethodDTOs
{
    public class UpdatePaymentMethodRequest
    {
        public string? PaymentMethod { get; set; }
        public bool IsValid() => !string.IsNullOrEmpty(PaymentMethod);
    }
}
