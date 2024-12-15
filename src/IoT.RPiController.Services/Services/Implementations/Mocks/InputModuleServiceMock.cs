using AutoMapper;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;

namespace IoT.RPiController.Services.Services.Implementations.Mocks
{
    public class InputModuleServiceMock(
        IModuleConfigurationRepository configRepository,
        IMapper mapper) : IInputModuleService
    {
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        public async Task<IEnumerable<InputModuleDto>> ReadStateAsync()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var modules = new List<InputModuleDto>();
                // TODO: replace IM with ModuleTypeEnum
                modules.AddRange((await configRepository.GetAllAsync())
                    .Where(x => x.ModuleType?.StartsWith("IM") == true)
                    .Select(c => mapper.Map<InputModuleDto>(c)));

                return modules;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<InputModuleDto> ReadStateByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var config = await configRepository.GetByIdAsync(id);
                var model = mapper.Map<InputModuleDto>(config);
                return model;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}