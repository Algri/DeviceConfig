using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IoT.RPiController.Data.Repositories.Implementations;

public class TimerValueRepository(RPiContext context) :
    Repository<TimerValue>(context), ITimerValueRepository
{
    public async Task<TimerValue?> GetByModuleIdAndRelayNumberAsync(int relayNumber, int moduleId) =>
        await AsQueryable().FirstOrDefaultAsync(x => x.RelayNumber == relayNumber && x.ModuleId == moduleId);

    public IQueryable<TimerValue> IncludeModules() => AsQueryable().Include(tv => tv.Module);

    public async Task<TimerValue?> GetByIdWithModuleAsync(int id)
    {
        var timerValue = await IncludeModules().FirstOrDefaultAsync(tV => tV.Id == id);

        if (timerValue?.Module != null)
        {
            timerValue.Module.TimerValues = new List<TimerValue>();
        }

        return timerValue;
    }

    public async Task<IEnumerable<TimerValue>?> GetAllWithModuleAsync() => (await IncludeModules().ToListAsync())
        .Select(tV =>
        {
            if (tV.Module != null)
            {
                tV.Module.TimerValues = new List<TimerValue>();
            }

            return tV;
        });
}