using System.Collections.Concurrent;
using IoT.RPiController.Data;
using IoT.RPiController.Data.Entities;
using IoT.RPiController.Data.Enums;
using IoT.RPiController.Data.Repositories.Abstractions;
using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Helpers;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations.Mocks
{
    public class RelayServiceMock(
        IEventService eventService,
        ITimerTokenService timerTokenService,
        ILogger<RelayServiceMock> logger,
        IServiceScopeFactory serviceScopeFactory)
        : IRelayService
    {
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _relayLocks = new();

        public async Task<bool> ReadRelayAsync(int address, int relayNumber)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var semaphoreSlim = _relayLocks.GetOrAdd(address, new SemaphoreSlim(1, 1));

            await semaphoreSlim.WaitAsync();
            try
            {
                var config = await GetModuleConfigurationAsync(address, scope);

                relayNumber = (byte)(relayNumber - 1);

                return relayNumber switch
                {
                    < 8 => IoTHelper.CheckBit(config.PortA, (byte)relayNumber),
                    >= 8 and < 16 => IoTHelper.CheckBit(config.PortB, (byte)(relayNumber - 8)),
                    _ => throw new ArgumentOutOfRangeException(nameof(relayNumber))
                };
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task WriteRelayAsync(int address, int relayNumber, bool value)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var semaphoreSlim = _relayLocks.GetOrAdd(address, new SemaphoreSlim(1, 1));

            await semaphoreSlim.WaitAsync();
            try
            {
                var config = await GetModuleConfigurationAsync(address, scope);

                relayNumber = (byte)(relayNumber - 1);
                switch (relayNumber)
                {
                    case < 8:
                    {
                        var updatedPortA = IoTHelper.UpdateByte(config.PortA, (byte)relayNumber, value);
                        config.PortA = updatedPortA;
                        break;
                    }
                    case >= 8 and < 16:
                    {
                        var updatedPortB = IoTHelper.UpdateByte(config.PortB, (byte)(relayNumber - 8), value);
                        config.PortB = updatedPortB;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(relayNumber));
                }

                await SaveChangesAsync(scope);
                await eventService.SendEvent(EventNameEnum.OnRelayStateUpdate, config);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task ToggleRelayAsync(RelayBaseDto relayBase)
        {
            var moduleAddress = (byte)relayBase.Address;
            var relayNumber = relayBase.RelayNumber;
            logger.LogWarning(relayNumber.ToString());

            using var scope = serviceScopeFactory.CreateScope();
            var moduleConfigurationRepository =
                scope.ServiceProvider.GetRequiredService<IModuleConfigurationRepository>();

            var module = await moduleConfigurationRepository.GetByAddressAsync(moduleAddress);
            if (module is null)
            {
                throw new ArgumentException("No module found for this id!");
            }

            try
            {
                var currentState = await ReadRelayAsync(moduleAddress, relayNumber);
                await WriteRelayAsync(moduleAddress, relayNumber, !currentState);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error on relay number [{relayNumber} toggle for module id: [{moduleAddress}]]", ex);
                throw;
            }
        }

        //TODO: refactor it, do we need it??
        private async Task<Module> GetModuleConfigurationAsync(int address, IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RPiContext>();
            var config = await dbContext.ModuleConfigurations
                .Include(m => m.TimerValues)
                .FirstOrDefaultAsync(m => m.Address == (byte)address);

            if (config is null)
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

            return config;
        }

        private async Task SaveChangesAsync(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RPiContext>();
            await dbContext.SaveChangesAsync();
        }
    }
}