using Repository.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRoleRepository
    {
        public Task<IEnumerable<Role>> GetAllRolesAsync();
        public Task<Role> GetRoleByNameAsync(string roleName);
        public Task<Role> CreateRoleAsync(string roleName);
        public Task<Role> GetRoleByIdAsync(int roleId);
        public Task<bool> DeleteRoleAsync(Role role);

    }
}
