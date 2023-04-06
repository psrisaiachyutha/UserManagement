using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models.Entities;

namespace Repository.Implementations
{

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role).SingleOrDefaultAsync(user => user.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles).SingleOrDefaultAsync(user => user.UserId == id);
        }

        public async Task<UserRole> AssignRoleAsync(int userId, int roleId)
        {
            UserRole userRole = new(){ RoleId = roleId, UserId = userId };
            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            return userRole;
        }

        public async Task<bool> DeleteAllUserRoles(int userId)
        {
            var userRoles = await _dbContext.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
            _dbContext.RemoveRange(userRoles);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
