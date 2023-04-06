using System.ComponentModel.DataAnnotations;

namespace Repository.Models.Entities
{
    public class User
    {
        public User()
        {
            this.UserRoles = new HashSet<UserRole>();
            //this.Roles = new HashSet<Role>();
        }
        
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;        
        //public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
