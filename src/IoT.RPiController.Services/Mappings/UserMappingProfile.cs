using AutoMapper;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ReverseMap();
            CreateMap<User, CreateOrUpdateUserDto>()
                .ReverseMap();
        }
    }
}
