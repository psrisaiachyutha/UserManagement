namespace Common.Models.Responses
{
    public class AssignRoleResponseDTO
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}
