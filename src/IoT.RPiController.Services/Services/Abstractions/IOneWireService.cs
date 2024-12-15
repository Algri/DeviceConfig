using IoT.RPiController.Services.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IOneWireService
    {
        Task ReadOneWireAsync(CancellationToken cancellationToken);
    }
}