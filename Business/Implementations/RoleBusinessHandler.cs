using AutoMapper;
using Business.Interfaces;
using Common.Constants;
using Common.Exceptions;
using Common.Models.Requests;
using Common.Models.Responses;
using Repository.Interfaces;

namespace Business.Implementations
{
    public class RoleBusinessHandler : IRoleBusinessHandler
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleBusinessHandler(
            IMapper mapper,
            IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return _mapper.Map<IEnumerable<RoleResponseDTO>>(roles);
        }

        public async Task<RoleResponseDTO> CreateRoleAsync(CreateRoleRequestDTO createRoleRequestDTO)
        {
            var role = await _roleRepository.GetRoleByNameAsync(createRoleRequestDTO.Name);
            if (role is not null)
            {
                throw new RecordAlreadyExistsException(ErrorMessages.RoleAlreadyExists(createRoleRequestDTO.Name));
            }

            var createdRole = await _roleRepository.CreateRoleAsync(createRoleRequestDTO.Name);
            return _mapper.Map<RoleResponseDTO>(createdRole);
        }

        public async Task<bool> DeleteRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role is null)
            {
                throw new RecordNotFoundException(ErrorMessages.RoleByIdNotFound(roleId));
            }
            return await _roleRepository.DeleteRoleAsync(role);
        }
    }
}
