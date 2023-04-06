#region References
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Repository.Models.Entities;
#endregion References

namespace Repository.Implementations
{
    /// <summary>
    /// Role repository to handle the entity Role
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        #region References
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RoleRepository> _logger;
        #endregion References

        /// <summary>
        /// RoleRepository constructor initialized with its dependents
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dbContext"></param>
        public RoleRepository(
            ILogger<RoleRepository> logger,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _dbContext.Roles
                .SingleOrDefaultAsync(role => role.Name == roleName);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Role> CreateRoleAsync(string roleName)
        {
            Role role = new() { Name = roleName };
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();

            return role;
        }

        /// <inheritdoc/>
        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            return await _dbContext.Roles
                .SingleOrDefaultAsync(role => role.RoleId == roleId);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteRoleAsync(Role role)
        {
            _dbContext.Remove(role);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
