using AutoMapper;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Mappings
{
    public class RelayModuleMappingProfile : Profile
    {
        public RelayModuleMappingProfile()
        {
            CreateMap<Module, RelayModuleDto>()
                .ReverseMap();
        }
    }
}
