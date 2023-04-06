#region References
using AutoMapper;
using Common.Models.Requests;
using Common.Models.Responses;
using Repository.Models.Entities;
#endregion References

namespace Business.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserRequestDTO, User>();

            CreateMap<User, UserResponseDTO>()
                .ForMember(x => x.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(x => x.Role)));

            CreateMap<AssignRoleRequestDTO, AssignRoleResponseDTO>().ReverseMap();
            
            CreateMap<RoleResponseDTO, Role>().ReverseMap();
        }
    }
}
