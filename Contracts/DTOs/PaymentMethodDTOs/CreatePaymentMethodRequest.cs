namespace Contracts.DTOs.PaymentMethodDTOs
{
    public class CreatePaymentMethodRequest
    {
        private int _ID;
        public int MethodID => _ID;
        public required string PaymentMethod { get; set; }
        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(PaymentMethod);
    }
}
