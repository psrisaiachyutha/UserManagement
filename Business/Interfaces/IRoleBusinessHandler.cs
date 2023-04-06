using Common.Models.Requests;
using Common.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IRoleBusinessHandler
    {
        public Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync();
        public Task<RoleResponseDTO> CreateRoleAsync(CreateRoleRequestDTO createRoleRequestDTO);
        public Task<bool> DeleteRoleByIdAsync(int roleId);
    }
}
