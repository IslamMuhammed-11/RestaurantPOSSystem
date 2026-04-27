namespace Contracts.DTOs.TableDTOs
{
    public class UpdateTableRequest
    {
        public string? Name { get; set; }
        public int? Seats { get; set; }
        public bool IsValid() => !string.IsNullOrEmpty(Name) || Seats.HasValue;
    }
}
