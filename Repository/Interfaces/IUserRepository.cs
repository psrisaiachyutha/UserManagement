using Repository.Models.Entities;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> CreateUserAsync(User user);
        public Task<IEnumerable<User>> GetAllUsersAsync();
        public Task<User> GetUserByIdAsync(int id);
        public Task<UserRole> AssignRoleAsync(int userId, int roleId);
        public Task<bool> DeleteAllUserRoles(int userId);
    }
}
