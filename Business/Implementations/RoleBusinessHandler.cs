#region References
using AutoMapper;
using Business.Interfaces;
using Common.Constants;
using Common.Exceptions;
using Common.ExtensionMethods;
using Common.Models.Requests;
using Common.Models.Responses;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
#endregion References

namespace Business.Implementations
{
    public class RoleBusinessHandler : IRoleBusinessHandler
    {
        #region Declarations
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleBusinessHandler> _logger;
        #endregion Declarations

        #region Constructor
        /// <summary>
        /// RoleBusinessHandler constructor is created with its dependents
        /// </summary>
        /// <param name="logger">for logging RoleBusinessHandler related data </param>
        /// <param name="mapper">for mapping the objects</param>
        /// <param name="roleRepository">for accessing the database for role entity</param>
        public RoleBusinessHandler(
            ILogger<RoleBusinessHandler> logger,
            IMapper mapper,
            IRoleRepository roleRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }
        #endregion Constructor

        /// <summary>
        /// This function which handles the buissiness things for fetching the all the roles present in 
        /// the database
        /// </summary>
        /// <returns>collection of roles</returns>
        public async Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync()
        {
            _logger.LogInformation("{functionName} is triggered", nameof(GetAllRolesAsync));
            
            var roles = await _roleRepository.GetAllRolesAsync();
            return _mapper.Map<IEnumerable<RoleResponseDTO>>(roles);
        }

        /// <summary>
        /// This function handles the business code for creating a new role
        /// </summary>
        /// <param name="createRoleRequestDTO"></param>
        /// <returns>newly created role</returns>
        /// <exception cref="RecordAlreadyExistsException">this exception will be raised when role is already exists in the database
        /// </exception>
        public async Task<RoleResponseDTO> CreateRoleAsync(CreateRoleRequestDTO createRoleRequestDTO)
        {
            _logger.LogInformation("{functionName} is triggered", nameof(CreateRoleAsync));

            var role = await _roleRepository.GetRoleByNameAsync(createRoleRequestDTO.Name);
            
            if (role is not null)
            {
                _logger.LogDebug("Role already exists in the database. Record : {role}", role.JsonSerialize());
                throw new RecordAlreadyExistsException(ErrorMessages.RoleAlreadyExists(createRoleRequestDTO.Name));
            }

            var createdRole = await _roleRepository.CreateRoleAsync(createRoleRequestDTO.Name);
            return _mapper.Map<RoleResponseDTO>(createdRole);
        }

        /// <summary>
        /// This function handles the business code for deleting a role based on id
        /// </summary>
        /// <param name="roleId">role id in the database</param>
        /// <returns>true when able to successfully delete the role</returns>
        /// <exception cref="RecordNotFoundException">when role record doesn't exists in the database</exception>
        public async Task<bool> DeleteRoleByIdAsync(int roleId)
        {
            _logger.LogInformation("{functionName} is triggered", nameof(DeleteRoleByIdAsync));

            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role is null)
            {
                _logger.LogDebug("Role already exists in the database. Record : {role}", role.JsonSerialize());
                throw new RecordNotFoundException(ErrorMessages.RoleByIdNotFound(roleId));
            }
            _logger.LogInformation("Fetched the role {role}", role.JsonSerialize());
            return await _roleRepository.DeleteRoleAsync(role);
        }
    }
}
