using System.ComponentModel.DataAnnotations;

namespace Repository.Models.Entities
{
    public class User
    {
        //TODO move this key to db context
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
