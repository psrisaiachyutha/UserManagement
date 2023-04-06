using Common.Models.Requests;
using Common.Models.Responses;

namespace Business.Interfaces
{
    public interface IUserBusinessHandler
    {
        public Task<TokenResponseDTO> VerifyLoginAsync(LoginRequestDTO loginRequest);
        public Task<UserResponseDTO> RegisterUserAsync(CreateUserRequestDTO createUserRequestDTO);
        public Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        public Task<AssignRoleResponseDTO> AssignRoleForUserAsync(AssignRoleRequestDTO assignRoleRequestDTO);
        public Task<bool> DeleteAllRoles(int userId);
    }
}
