using AutoMapper;
using Business.Interfaces;
using Common.Configurations;
using Common.Constants;
using Common.Constants.Enums;
using Common.Exceptions;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using Repository.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Business.Implementations
{

    public class UserBusinessHandler : IUserBusinessHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly ApplicationSettings _applicationSettings;


        public UserBusinessHandler(
            IMapper mapper,
            IUserRepository userRepository,
            IOptions<ApplicationSettings> applicationSettings)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _applicationSettings = applicationSettings.Value;
            
        }

        public async Task<TokenResponse> VerifyLoginAsync(LoginRequestDTO loginRequest)
        {
            // Fetching the user based on email 
            var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

            if (user is null)
            {
                throw new RecordNotFoundException(ErrorMessages.UserNotFound(loginRequest.Email));
            }

            if (!VerifyPasswordHash(loginRequest.Password, user.PasswordHash))
            {
                throw new InvalidCredentialsException(ErrorMessages.EmailOrPasswordIncorrect);
            }

            return new TokenResponse { Token = GenerateJwtToken(user) };


        }

        public async Task<UserResponseDTO> RegisterUserAsync(CreateUserRequestDTO createUserRequestDTO)
        {

            // Fetching the user based on email 
            var user = await _userRepository.GetUserByEmailAsync(createUserRequestDTO.Email);

            if (user is not null)
            {
                throw new RecordAlreadyExistsException(ErrorMessages.UserAlreadyExists(createUserRequestDTO.Email));
            }

            User userToBeCreated = _mapper.Map<User>(createUserRequestDTO);
            userToBeCreated.PasswordHash = CreatePasswordHash(createUserRequestDTO.Password);


            var createdUser = await _userRepository.CreateUserAsync(userToBeCreated);

            return _mapper.Map<UserResponseDTO>(createdUser);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        #region Private methods
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, nameof(RolesEnum.Admin)),
                new Claim(ClaimTypes.Role, nameof(RolesEnum.Manager)),
                new Claim(ClaimTypes.Role, nameof(RolesEnum.User))

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_applicationSettings.SignatureKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string CreatePasswordHash(string password)
        {
            //using var hmac = new HMACSHA512(Convert.FromBase64String(_configuration["Jwt:SecretKey"]));
            //using var hmac = new HMACSHA512(Convert.FromBase64String(_applicationSettings.PasswordEncriptionKey));
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_applicationSettings.PasswordEncriptionKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPasswordHash(string password, string hash)
        {
            //using var hmac = new HMACSHA512(Convert.FromBase64String(_configuration["Jwt:SecretKey"]));
            //using var hmac = new HMACSHA512(Convert.FromBase64String(_applicationSettings.PasswordEncriptionKey));
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_applicationSettings.PasswordEncriptionKey));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(Convert.FromBase64String(hash));
        }
        #endregion Private methods

    }
}
