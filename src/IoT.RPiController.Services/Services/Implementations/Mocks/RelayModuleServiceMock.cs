using AutoMapper;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;

namespace IoT.RPiController.Services.Services.Implementations.Mocks
{
    public class RelayModuleServiceMock(
        IEventService eventService,
        IModuleConfigurationRepository moduleConfigurationRepository,
        IMapper mapper)
        : IRelayModuleService
    {
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        public async Task<IEnumerable<RelayModuleDto>> ReadStateAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var relayModules = new List<RelayModuleDto>();
                foreach (var module in await moduleConfigurationRepository.GetAllAsync())
                {
                    var model = mapper.Map<RelayModuleDto>(module);
                    relayModules.Add(model);
                }

                return relayModules;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<RelayModuleDto> ReadStateByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var module = await moduleConfigurationRepository.GetByIdAsync(id);
                var model = mapper.Map<RelayModuleDto>(module);
                return model;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IEnumerable<RelayModuleDto>> WriteStateAsync(IEnumerable<RelayModuleDto> relayModules)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                foreach (var relayModule in relayModules)
                {
                    var module = await moduleConfigurationRepository.GetByAddressAsync(relayModule.Address);
                    if (module is null)
                    {
                        continue;
                    }

                    module.PortA = relayModule.PortA;
                    module.PortB = relayModule.PortB;

                    await moduleConfigurationRepository.SaveChangesAsync();
                    await eventService.SendEvent(EventNameEnum.OnModuleStateUpdate, module);
                }

                return relayModules;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}