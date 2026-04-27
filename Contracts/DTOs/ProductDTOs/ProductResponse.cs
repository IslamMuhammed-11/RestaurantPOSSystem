namespace Contracts.DTOs.ProductDTOs
{
    public class ProductResponse
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
