using Common.Models.Requests;
using Common.Models.Responses;

namespace Business.Interfaces
{
    public interface IRoleBusinessHandler
    {
        public Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync();

        public Task<RoleResponseDTO> CreateRoleAsync(CreateRoleRequestDTO createRoleRequestDTO);

        public Task<bool> DeleteRoleByIdAsync(int roleId);
    }
}
