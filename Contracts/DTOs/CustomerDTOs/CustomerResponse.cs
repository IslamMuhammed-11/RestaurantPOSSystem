namespace Contracts.DTOs.CustomerDTOs
{
    public class CustomerResponse
    {
        public int CustomerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
