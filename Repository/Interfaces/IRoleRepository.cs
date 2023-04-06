using Repository.Models.Entities;

namespace Repository.Interfaces
{
    public interface IRoleRepository
    {
        /// <summary>
        /// Fetches all roles from database
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Role>> GetAllRolesAsync();

        /// <summary>
        /// Gets role based on name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Task<Role> GetRoleByNameAsync(string roleName);

        /// <summary>
        /// Creates a new Role record in the database
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Task<Role> CreateRoleAsync(string roleName);

        /// <summary>
        /// Gets role based on the id 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<Role> GetRoleByIdAsync(int roleId);
        
        /// <summary>
        /// Deletes the role from database
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> DeleteRoleAsync(Role role);
    }
}
