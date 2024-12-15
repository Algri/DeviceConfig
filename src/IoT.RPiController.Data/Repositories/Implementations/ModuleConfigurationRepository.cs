using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Data.Repositories.Implementations;

public class ModuleConfigurationRepository(RPiContext context, ITimerValueRepository timerValueRepository)
    : Repository<Module>(context), IModuleConfigurationRepository
{
    public async Task<Module?> GetByAddressAsync(byte address) => (await GetAllFullIncludedAsync()).FirstOrDefault(m => m.Address == address);

    public async Task<List<Module>> GetAllNextByAddressAsync(byte address) => await AsQueryable().Where(x => x.Address > address).ToListAsync();

    public async Task<List<Module>> GetAllFullIncludedAsync() => await AsQueryable()
        .Include(x => x.RelayInfos)
        .Include(y => y.TimerValues)
        .OrderBy(m => m.Address)
        .ToListAsync();
}