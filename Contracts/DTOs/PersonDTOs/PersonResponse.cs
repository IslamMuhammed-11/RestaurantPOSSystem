namespace Contracts.DTOs.PersonDTOs
{
    public class PersonResponse
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
