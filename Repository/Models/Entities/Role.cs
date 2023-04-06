namespace Repository.Models.Entities
{
    public class Role
    {
        public Role()
        {
            //Users = new HashSet<User>();
        }
        
        public int RoleId { get; set; }
        public string Name { get; set; }
        
        //public virtual ICollection<User> Users { get; set; }
        public  ICollection<UserRole> UserRoles { get; set; }
    }
}
