using Repository.Models.Entities;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> CreateUserAsync(User user);
        public Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
