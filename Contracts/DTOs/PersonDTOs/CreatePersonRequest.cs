namespace Contracts.DTOs.PersonDTOs
{
    public class CreatePersonRequest
    {
        private int _ID;
        public int PersonID => _ID;
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }

        public void SetID(int id) => _ID = id;

        public bool IsValid() => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(SecondName);
    }
}