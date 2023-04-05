using Common.Models.Requests;
using Common.Models.Responses;

namespace Business.Interfaces
{
    public interface IUserBusinessHandler
    {
        public Task<TokenResponse> VerifyLoginAsync(LoginRequestDTO loginRequest);
        public Task<UserResponseDTO> RegisterUserAsync(CreateUserRequestDTO createUserRequestDTO);
        public Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    }
}
