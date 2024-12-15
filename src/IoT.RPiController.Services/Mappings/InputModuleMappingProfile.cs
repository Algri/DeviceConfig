using AutoMapper;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Mappings
{
    public class InputModuleMappingProfile : Profile
    {
        public InputModuleMappingProfile()
        {
            CreateMap<Module, InputModuleDto>()
                .ReverseMap();
        }
    }
}
