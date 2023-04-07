#region References
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Repository.Models.Entities;
#endregion References

namespace Repository.Implementations
{
    /// <summary>
    /// UserRepository to handle entity User
    /// </summary>
    public class UserRepository : IUserRepository
    {
        #region References
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;
        #endregion References

        /// <summary>
        /// UserRepository constructor is created with its dependents
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbContext"></param>
        public UserRepository(
            ILogger<UserRepository> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role).SingleOrDefaultAsync(user => user.Email == email);
        }

        /// <inheritdoc/>
        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return user;
            }
            catch (InvalidOperationException ex )
            {
                throw ex;
            }
            
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles).SingleOrDefaultAsync(user => user.UserId == id);
        }

        /// <inheritdoc/>
        public async Task<UserRole> AssignRoleAsync(int userId, int roleId)
        {
            UserRole userRole = new(){ RoleId = roleId, UserId = userId };
            await _dbContext.UserRoles.AddAsync(userRole);
            await _dbContext.SaveChangesAsync();

            return userRole;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAllUserRoles(int userId)
        {
            var userRoles = await _dbContext.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
            _dbContext.RemoveRange(userRoles);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
