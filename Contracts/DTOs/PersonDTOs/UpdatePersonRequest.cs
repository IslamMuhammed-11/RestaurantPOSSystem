namespace Contracts.DTOs.PersonDTOs
{
    public class UpdatePersonRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public bool IsValid() => !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) || !string.IsNullOrEmpty(Phone);
    }
}
