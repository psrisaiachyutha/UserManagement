using System.Text.Json.Serialization;

namespace Repository.Models.Entities
{
    public class User
    {
        public User()
        {
            this.UserRoles = new HashSet<UserRole>();
        }
        
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        [JsonIgnore]
        public string PasswordHash { get; set; } = null!;                
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
