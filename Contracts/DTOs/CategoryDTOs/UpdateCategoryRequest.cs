namespace Contracts.DTOs.CategoryDTOs
{
    public class UpdateCategoryRequest
    {
        public string? Name { get; set; }
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
