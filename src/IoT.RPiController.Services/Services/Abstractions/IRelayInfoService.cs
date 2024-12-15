using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions;

public interface IRelayInfoService
{
    Task<IEnumerable<RelayInfo>?> GetAllAsync();
    Task<RelayInfo?> GetByIdAsync(int id);
    Task<IEnumerable<RelayInfo>> AddBatchAsync(IEnumerable<RelayInfoDto> relayInfos);
    Task<IEnumerable<RelayInfo>?> UpdateBatchAsync(IEnumerable<RelayInfoDto> relayInfos);
    Task<bool> DeleteBatchAsync(IEnumerable<int> id);
}