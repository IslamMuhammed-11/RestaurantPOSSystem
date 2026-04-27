namespace Contracts.DTOs.CustomerDTOs
{
    public class CreateCustomerRequest
    {
        private int _ID;
        public int CustomerID => _ID;
        public required string Name { get; set; }
        public string? Phone { get; set; }
        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
