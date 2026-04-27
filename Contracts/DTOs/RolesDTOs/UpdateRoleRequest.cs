namespace Contracts.DTOs.RolesDTOs
{
    public class UpdateRoleRequest
    {
        public string? Name { get; set; }
        public bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}
