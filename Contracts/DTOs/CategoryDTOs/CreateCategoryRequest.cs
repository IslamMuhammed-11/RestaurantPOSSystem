namespace Contracts.DTOs.CategoryDTOs
{
    public class CreateCategoryRequest
    {
        private int _ID;
        public int CategoryID { get { return _ID; } }
        public required string Name { get; set; }
        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
