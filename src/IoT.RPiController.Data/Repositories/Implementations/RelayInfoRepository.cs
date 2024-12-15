using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Data.Repositories.Implementations;

public class RelayInfoRepository(RPiContext context) : Repository<RelayInfo>(context), IRelayInfoRepository
{
    public IQueryable<RelayInfo> IncludeModule() => AsQueryable().Include(r => r.Module);


    public async Task<RelayInfo?> GetByIdWithModuleAsync(int id)
    {
        var relayInfo = await IncludeModule().FirstOrDefaultAsync(r => r.Id == id);

        if (relayInfo?.Module != null)
        {
            relayInfo.Module.RelayInfos = new List<RelayInfo>();
        }

        return relayInfo;
    }

    public async Task<IEnumerable<RelayInfo>?> GetAllWithModuleAsync() => (await IncludeModule().ToListAsync())
        .Select(relayInfo =>
        {
            if (relayInfo.Module != null)
            {
                relayInfo.Module.RelayInfos = new List<RelayInfo>();
            }

            return relayInfo;
        });
}