using IoT.RPiController.Data.Entities;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.RPiController.Services.Services.Implementations;

public class RelayService : IRelayService
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    public async Task<bool> ReadRelayAsync(int address, int relayNumber)
    {
        throw new NotImplementedException();
    }

    public async Task WriteRelayAsync(int address, int relayNumber, bool value)
    {
        throw new NotImplementedException();
    }

    public async Task ToggleRelayAsync(RelayBaseDto relayBase)
    {
        throw new NotImplementedException();
    }

    public async Task RelaySwitchOnTimerAsync(RelayStateTimerDto relayState)
    {
        throw new NotImplementedException();
    }

    private async Task SwitchRelayOnTimerAsync(RelayStateTimerDto relayState, int timerValueId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task DelayRelayOnTimerAsync(RelayStateTimerDto relayState, int timerValueId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    private async Task<Module> GetModuleConfigurationAsync(int address, IServiceScope scope)
    {
        throw new NotImplementedException();
    }

    private async Task SaveChangesAsync(IServiceScope scope)
    {
        throw new NotImplementedException();
    }
}