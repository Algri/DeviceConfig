using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using AutoMapper;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Services.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations
{
    public class InputModuleService(
        IMemoryCache memoryCache,
        IModuleConfigurationRepository moduleConfigurationRepository,
        ILogger<InputModuleService> logger,
        IMapper mapper)
        : IInputModuleService
    {
        public async Task<IEnumerable<InputModuleDto>> ReadStateAsync()
        {
            var inputModuleDtos = (await moduleConfigurationRepository.GetAllFullIncludedAsync())
                .Where(x => x.ModuleType?.StartsWith("IM") == true)
                .ToList();
            inputModuleDtos.ForEach(inputModuleDto =>
            {
                var key = GetXdiCacheKey(DeviceType.xDI, inputModuleDto.Address);
                memoryCache.TryGetValue(key, out object? state);
                inputModuleDto.PortA = (byte)Convert.ToInt32(state);
            });

            return mapper.Map<List<InputModuleDto>>(inputModuleDtos);
        }

        public async Task<InputModuleDto> ReadStateByIdAsync(int id)
        {
            var module = (await moduleConfigurationRepository.GetAllFullIncludedAsync()).FirstOrDefault(m => m.Id == id);
            if (module == null)
            {
                var msg = $"InputModule is not found for id: {id}";

                logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            var key = GetXdiCacheKey(DeviceType.xDI, module.Address);
            memoryCache.TryGetValue(key, out object? state);

            module.PortA = (byte)Convert.ToInt32(state);
            return mapper.Map<InputModuleDto>(module);
        }

        private string GetXdiCacheKey(DeviceType deviceType, int address) =>
        string.Join('_', [deviceType, address]);
    }
}
