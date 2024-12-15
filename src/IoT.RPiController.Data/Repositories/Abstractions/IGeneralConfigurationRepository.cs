using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Enums;

namespace IoT.RPiController.Data.Repositories.Abstractions;

public interface IGeneralConfigurationRepository : IRepository<GeneralConfiguration>
{
    public Task<GeneralConfiguration?> GetByKeyAsync(ConfigurationKeysEnum key);
    public Task<T?> GetTypedValueByKeyAsync<T>(ConfigurationKeysEnum key);
}