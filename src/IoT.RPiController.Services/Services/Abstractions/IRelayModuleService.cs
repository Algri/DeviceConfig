using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IRelayModuleService
    {
        Task<RelayModuleDto> ReadStateByIdAsync(int id);
        Task<IEnumerable<RelayModuleDto>> ReadStateAsync();
        Task<IEnumerable<RelayModuleDto>> WriteStateAsync(IEnumerable<RelayModuleDto> modules);
    }
}
