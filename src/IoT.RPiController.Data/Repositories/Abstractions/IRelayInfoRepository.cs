using IoT.RPiController.Data.Entities;

namespace IoT.RPiController.Data.Repositories.Abstractions;

public interface IRelayInfoRepository : IRepository<RelayInfo>
{
    IQueryable<RelayInfo> IncludeModule();
    Task<RelayInfo?> GetByIdWithModuleAsync(int id);
    Task<IEnumerable<RelayInfo>?> GetAllWithModuleAsync();
}