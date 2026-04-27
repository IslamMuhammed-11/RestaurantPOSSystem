namespace Contracts.DTOs.CustomerDTOs
{
    public class UpdateCustomerRequest
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
