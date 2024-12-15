using IoT.RPiController.Services.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IRelayService
    {
        Task<bool> ReadRelayAsync(int address, int relayNumber);
        Task WriteRelayAsync(int address, int relayNumber, bool value);
        Task ToggleRelayAsync(RelayBaseDto relayBaseDto);
    }
}