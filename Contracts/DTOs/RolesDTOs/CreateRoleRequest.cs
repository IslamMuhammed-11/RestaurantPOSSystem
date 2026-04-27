namespace Contracts.DTOs.RolesDTOs
{
    public class CreateRoleRequest
    {
        private int _ID;
        public int RoleID => _ID;
        public required string Name { get; set; }
        public void SetID(int id) => _ID = id;
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
