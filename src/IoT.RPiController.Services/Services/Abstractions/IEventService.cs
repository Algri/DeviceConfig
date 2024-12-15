using IoT.RPiController.Services.Enums;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IEventService
    {
        Task SendEvent(EventNameEnum eventName, object args);
        Task SendEvent(EventNameEnum eventName, object args, CancellationToken cancellationToken);
    }
}