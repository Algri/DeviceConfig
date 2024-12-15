using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IInputModuleService
    {
        Task<InputModuleDto> ReadStateByIdAsync(int id);
        Task<IEnumerable<InputModuleDto>> ReadStateAsync();
    }
}
