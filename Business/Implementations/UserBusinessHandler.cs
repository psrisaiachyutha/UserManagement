﻿#region References
using AutoMapper;
using Business.Interfaces;
using Common.Constants;
using Common.Exceptions;
using Common.ExtensionMethods;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Repository.Models.Entities;
#endregion References

namespace Business.Implementations
{
    public class UserBusinessHandler : IUserBusinessHandler
    {
        #region Declarations
        private readonly ILogger<UserBusinessHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IJWTHelper _jWTHelper;
        #endregion Delcarations

        #region Constructor
        /// <summary>
        /// UserBusinessHandler constructor is created with its dependents
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="userRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="jWTHelper"></param>
        public UserBusinessHandler(
            ILogger<UserBusinessHandler> logger,
            IMapper mapper,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IJWTHelper jWTHelper)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jWTHelper = jWTHelper;
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// This function handles the business code for verifying the login request of user
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>token if credentials are correct</returns>
        /// <exception cref="RecordNotFoundException">When user doesn't exists with the requested email</exception>
        /// <exception cref="InvalidCredentialsException">when password is wrong for the requested email</exception>
        public async Task<TokenResponseDTO> VerifyLoginAsync(LoginRequestDTO loginRequest)
        {
            _logger.LogInformation("{functionName} is triggered", nameof(VerifyLoginAsync));

            var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

            if (user is null)
            {
                _logger.LogError("User doesn't exists with email: {email}", loginRequest.Email);
                throw new RecordNotFoundException(ErrorMessages.UserNotFound(loginRequest.Email));
            }

            _logger.LogInformation("user fetched from database, user record is {user}", user.JsonSerialize());

            if (!_jWTHelper.VerifyPasswordHash(loginRequest.Password, user.PasswordHash))
            {
                _logger.LogError("User credentials passed or wrong for email: {email}", loginRequest.Email);
                throw new InvalidCredentialsException(ErrorMessages.EmailOrPasswordIncorrect);
            }

            return new TokenResponseDTO { AccessToken = _jWTHelper.GenerateJwtToken(user) };

        }

        /// <summary>
        /// This function hanldes the business code for registering user in database
        /// </summary>
        /// <param name="createUserRequestDTO"></param>
        /// <returns>Newly created user</returns>
        /// <exception cref="RecordAlreadyExistsException">If user already exists in database</exception>
        public async Task<UserResponseDTO> RegisterUserAsync(CreateUserRequestDTO createUserRequestDTO)
        {
            _logger.LogInformation("{functionName} is triggered", nameof(RegisterUserAsync));

            var user = await _userRepository.GetUserByEmailAsync(createUserRequestDTO.Email);

            if (user is not null)
            {
                _logger.LogDebug("User with email: {email} already exists", createUserRequestDTO.Email);
                throw new RecordAlreadyExistsException(ErrorMessages.UserAlreadyExists(createUserRequestDTO.Email));
            }

            User userToBeCreated = _mapper.Map<User>(createUserRequestDTO);
            userToBeCreated.PasswordHash = _jWTHelper.CreatePasswordHash(createUserRequestDTO.Password);

            var createdUser = await _userRepository.CreateUserAsync(userToBeCreated);
            _logger.LogInformation("User created successfully");

            return _mapper.Map<UserResponseDTO>(createdUser);
        }

        /// <summary>
        /// This function handles business code for fetching all users from database 
        /// </summary>
        /// <returns>collection of all users from database </returns>
        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("{functionName} is triggered", nameof(GetAllUsersAsync));

            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        /// <summary>
        /// This function will handle business code for assigning role for user
        /// </summary>
        /// <param name="assignRoleRequestDTO">data to assign role</param>
        /// <returns>assigned role for the user</returns>
        /// <exception cref="RecordNotFoundException">when user or role not present in the database</exception>
        /// <exception cref="RecordAlreadyExistsException">when role already assigned for the user</exception>
        public async Task<AssignRoleResponseDTO> AssignRoleForUserAsync(AssignRoleRequestDTO assignRoleRequestDTO)
        {
            _logger.LogInformation("{functionName} is triggered", nameof(AssignRoleForUserAsync));

            var user = await _userRepository.GetUserByEmailAsync(assignRoleRequestDTO.Email);

            if (user is null)
            {
                _logger.LogError("User doesn't exists with email: {email}", assignRoleRequestDTO.Email);
                throw new RecordNotFoundException(ErrorMessages.UserNotFound(assignRoleRequestDTO.Email));
            }
            
            var role = await _roleRepository.GetRoleByNameAsync(assignRoleRequestDTO.RoleName);
            if (role is null)
            {
                _logger.LogError("Role doesn't exists will role: {role}", assignRoleRequestDTO.RoleName);
                throw new RecordNotFoundException(ErrorMessages.RoleNotFound(assignRoleRequestDTO.RoleName));
            }

            if (user.UserRoles.Select(x => x.RoleId).Contains(role.RoleId))
            {
                _logger.LogError("Role already assigned for the user");
                throw new RecordAlreadyExistsException(ErrorMessages.UserAlreadyContainsRole(user.Email, role.Name));
            }

            var userRole = await _userRepository.AssignRoleAsync(user.UserId, role.RoleId);

            var assignRoleResponseDTO = _mapper.Map<AssignRoleResponseDTO>(assignRoleRequestDTO);
            
            assignRoleResponseDTO.UserId = user.UserId;
            assignRoleResponseDTO.RoleId = role.RoleId;

            _logger.LogInformation("Role successfully assigned for the user");
            return assignRoleResponseDTO;
        }

        /// <summary>
        /// This function handles business code for deleting all roles related to a user
        /// </summary>
        /// <param name="userId">user id </param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException">when user doesnot exists or when user doesn't contain any roles</exception>
        public async Task<bool> DeleteAllRoles(int userId)
        {
            _logger.LogInformation("{functionName} is triggered", nameof(DeleteAllRoles));

            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                _logger.LogError("User with id: {id} doesn't exists in database", userId);
                throw new RecordNotFoundException(ErrorMessages.UserNotFoundWithId(userId));
            }
            if (!user.UserRoles.Any())
            {
                _logger.LogError("User doesn't contain any roles");
                throw new RecordNotFoundException(ErrorMessages.NoUserRolesFound(userId));
            }
            return await _userRepository.DeleteAllUserRoles(userId);
        }
        #endregion Public Methods
    }
}
