namespace Contracts.DTOs.ProductDTOs
{
    public class UpdateProductRequest
    {
        public int? CategoryID { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }

        public bool IsValid() => CategoryID.HasValue || !string.IsNullOrEmpty(Name) || Price.HasValue || IsAvailable.HasValue;
    }
}
