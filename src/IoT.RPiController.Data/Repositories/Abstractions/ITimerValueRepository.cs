using IoT.RPiController.Data.Entities;

namespace IoT.RPiController.Data.Repositories.Abstractions;

public interface ITimerValueRepository : IRepository<TimerValue>
{
    Task<TimerValue?> GetByModuleIdAndRelayNumberAsync(int relayNumber, int moduleId);
    IQueryable<TimerValue> IncludeModules();
    Task<TimerValue?> GetByIdWithModuleAsync(int id);
    Task<IEnumerable<TimerValue>?> GetAllWithModuleAsync();
}