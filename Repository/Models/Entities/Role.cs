namespace Repository.Models.Entities
{
    public class Role
    {
        public Role()
        {
            this.UserRoles = new HashSet<UserRole>();
        }
        
        public int RoleId { get; set; }
        public string Name { get; set; }
        
        public  ICollection<UserRole> UserRoles { get; set; }
    }
}
