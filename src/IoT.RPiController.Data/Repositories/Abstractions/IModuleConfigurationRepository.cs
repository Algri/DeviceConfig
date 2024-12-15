using IoT.RPiController.Data.Entities;

namespace IoT.RPiController.Data.Repositories.Abstractions;

public interface IModuleConfigurationRepository : IRepository<Module>
{
    Task<Module?> GetByAddressAsync(byte address);
    Task<List<Module>> GetAllNextByAddressAsync(byte address);
    Task<List<Module>> GetAllFullIncludedAsync();
}