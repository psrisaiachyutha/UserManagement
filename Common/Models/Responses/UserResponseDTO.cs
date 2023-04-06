namespace Common.Models.Responses
{
    public class UserResponseDTO
    {
        public UserResponseDTO()
        {
            Roles = new List<RoleResponseDTO>();
        }
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!; 
        public IList<RoleResponseDTO> Roles { get; set; }
    }
}
