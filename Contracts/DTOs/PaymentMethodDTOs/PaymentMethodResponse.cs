namespace Contracts.DTOs.PaymentMethodDTOs
{
    public class PaymentMethodResponse
    {
        public int MethodID { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
