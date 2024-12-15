using AutoMapper;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations;

public class RelayInfoService : IRelayInfoService
{
    private readonly IRelayInfoRepository _relayInfoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RelayInfoService> _logger;

    private readonly IModuleConfigurationRepository _moduleConfigurationRepository;

    public RelayInfoService(IRelayInfoRepository relayInfoRepository, ILogger<RelayInfoService> logger,
        IModuleConfigurationRepository moduleConfigurationRepository)
    {
        _relayInfoRepository = relayInfoRepository;
        _logger = logger;
        _moduleConfigurationRepository = moduleConfigurationRepository;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RelayInfoDto, RelayInfo>()
                .ForMember(dest => dest.Module, opt => opt.Ignore());
        });
        _mapper = mapperConfig.CreateMapper();
    }


    public async Task<IEnumerable<RelayInfo>?> GetAllAsync() => await _relayInfoRepository.GetAllWithModuleAsync();

    public async Task<RelayInfo?> GetByIdAsync(int id) => await _relayInfoRepository.GetByIdWithModuleAsync(id);

    public async Task<IEnumerable<RelayInfo>> AddBatchAsync(IEnumerable<RelayInfoDto> relayInfos)
    {
        var relayInfosToAdd = _mapper.Map<List<RelayInfo>>(relayInfos);

        foreach (var relayInfo in relayInfosToAdd)
        {
            var sameRelayNumberInfo = (await _relayInfoRepository.GetAllAsync())
                .FirstOrDefault(rInfo => rInfo.ModuleId == relayInfo.ModuleId && rInfo.RelayNumber == relayInfo.RelayNumber);

            if (sameRelayNumberInfo is not null)
            {
                const string msg = $"You shouldn't assign second description to the existing relay number info. ";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }

            var module = (await _moduleConfigurationRepository.GetAllFullIncludedAsync())
                .FirstOrDefault(x => x.Id == relayInfo.ModuleId);

            if (module is null)
            {
                var msg = $"Module is null for relay info. Module id: {relayInfo.ModuleId}";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }

            if (!Enum.TryParse<ModuleTypeEnum>(module.ModuleType, out var currentModuleType))
            {
                var msg = $"Module type parsing error. Module id: {relayInfo.ModuleId}";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }

            if (relayInfo.RelayNumber > currentModuleType.PortsAmount())
            {
                var msg = $"Relay info uses the relay number bigger than the available one. " +
                          $"Relay number: {relayInfo.RelayNumber} for Module type: {module.ModuleType}.";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }
        }

        await _relayInfoRepository.AddRangeAsync(relayInfosToAdd);
        await _relayInfoRepository.SaveChangesAsync();

        var ids = relayInfosToAdd.Select(tv => tv.Id);

        _logger.LogInformation($"New Relay Infos added with id: {string.Join(", ", ids)}");

        return relayInfosToAdd;
    }

    public async Task<IEnumerable<RelayInfo>?> UpdateBatchAsync(IEnumerable<RelayInfoDto> relayInfos)
    {
        var relayInfoDtos = relayInfos.ToList();

        foreach (var currentRelayInfo in relayInfoDtos)
        {
            var id = currentRelayInfo.Id;
            var existingRelayInfo = await GetByIdAsync(id);
            if (existingRelayInfo == null)
            {
                var msg = $"No existing Info found for id: {id}";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }

            if (existingRelayInfo.RelayNumber != currentRelayInfo.RelayNumber)
            {
                var info = currentRelayInfo;
                var sameRelayNumberInfo = (await _relayInfoRepository.GetAllAsync())
                    .FirstOrDefault(rInfo => rInfo.ModuleId == info.ModuleId && rInfo.RelayNumber == info.RelayNumber);

                if (sameRelayNumberInfo is not null)
                {
                    const string msg = $"You shouldn't assign second description to the exisiting relay nubmer info. ";

                    _logger.LogError(msg);
                    throw new ArgumentNullException(msg);
                }
            }

            if (!Enum.TryParse<ModuleTypeEnum>(existingRelayInfo.Module.ModuleType, out var currentModuleType))
            {
                var msg = $"Module type parsing error. Module id: {existingRelayInfo.ModuleId}";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }

            if (currentRelayInfo.RelayNumber > currentModuleType.PortsAmount())
            {
                var msg = $"Relay info uses the relay number bigger than the available one. " +
                          $"Relay number: {relayInfoDtos[id].RelayNumber} for Module type: {existingRelayInfo.Module.ModuleType}.";

                _logger.LogError(msg);
                throw new ArgumentNullException(msg);
            }

            _relayInfoRepository.Detach(existingRelayInfo);
        }

        var relayInfosToUpdate = _mapper.Map<List<RelayInfo>>(relayInfos);


        relayInfosToUpdate.ForEach(rInfo => { _relayInfoRepository.Update(rInfo); });

        await _relayInfoRepository.SaveChangesAsync();

        _logger.LogInformation("Existing Relay Infos updated");

        return relayInfosToUpdate;
    }

    public async Task<bool> DeleteBatchAsync(IEnumerable<int> ids)
    {
        var relayInfosToDelete = (await _relayInfoRepository.GetAllAsync()).Where(r => ids.Contains(r.Id)).ToList();
        if (relayInfosToDelete.Count == 0)
        {
            return false;
        }

        foreach (var relayInfo in relayInfosToDelete)
        {
            _relayInfoRepository.Delete(relayInfo);
        }

        await _relayInfoRepository.SaveChangesAsync();
        return true;
    }
}