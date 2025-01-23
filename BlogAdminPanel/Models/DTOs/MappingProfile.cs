using AutoMapper;

namespace BlogAdminPanel.Models.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map from UserCreateDto to User
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Image, opt => opt.Ignore()); // Ignore Image property during mapping

            // Map from User to UserCreateDto
            CreateMap<User, UserCreateDto>()
                .ForMember(dest => dest.Image, opt => opt.Ignore()); // Ignore Image property when mapping back to DTO

            // Other mappings
            CreateMap<User, UserUpdateDto>().ReverseMap();
        }
    }

}
