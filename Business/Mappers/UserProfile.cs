using AutoMapper;
using Common.Models.Requests;
using Common.Models.Responses;
using Repository.Models.Entities;

namespace Business.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserRequestDTO, User>();

            CreateMap<User, UserResponseDTO>();
        }
    }
}
