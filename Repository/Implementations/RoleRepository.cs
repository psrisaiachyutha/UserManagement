using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models.Entities;

namespace Repository.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _dbContext.Roles
                .SingleOrDefaultAsync(role => role.Name == roleName);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<Role> CreateRoleAsync(string roleName)
        {
            Role role = new() { Name = roleName };
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();

            return role;
        }

        public async Task<Role> GetRoleByIdAsync(int roleId)
        {
            return await _dbContext.Roles
                .SingleOrDefaultAsync(role => role.RoleId == roleId);
        }

        public async Task<bool> DeleteRoleAsync(Role role)
        {
            _dbContext.Remove(role);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
