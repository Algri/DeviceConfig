using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Enums;
using IoT.RPiController.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Data.Repositories.Implementations;

public class GeneralConfigurationRepository(RPiContext context) : Repository<GeneralConfiguration>(context), IGeneralConfigurationRepository
{
    public async Task<GeneralConfiguration?> GetByKeyAsync(ConfigurationKeysEnum key) =>
        await AsQueryable().FirstOrDefaultAsync(r => r.Key == key.ToString());

    public async Task<T?> GetTypedValueByKeyAsync<T>(ConfigurationKeysEnum key)
    {
        var config = await AsQueryable().FirstOrDefaultAsync(c => c.Key == key.ToString());

        if (config == null)
            return default;

        if (typeof(T).FullName != config.Type)
            throw new Exception($"Type mismatch: expected {typeof(T).FullName}, got {config.Type}");

        return (T)Convert.ChangeType(config.Value, typeof(T));
    }
}