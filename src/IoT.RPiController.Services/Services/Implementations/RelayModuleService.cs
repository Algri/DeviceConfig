using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;

namespace IoT.RPiController.Services.Services.Implementations
{
    public class RelayModuleService : IRelayModuleService
    {
        private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        public async Task<IEnumerable<RelayModuleDto>> ReadStateAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RelayModuleDto>> WriteStateAsync(IEnumerable<RelayModuleDto> modules)
        {
            throw new NotImplementedException();
        }

        public async Task<RelayModuleDto> ReadStateByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}