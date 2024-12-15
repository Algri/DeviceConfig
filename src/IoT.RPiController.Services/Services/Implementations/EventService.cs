using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Hubs;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IHubContext<IoTHub> _hubContext;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly ILogger<EventService> _logger;
        private readonly string eventNamespace = "IoT.RPiController";

        public EventService(
            IHubContext<IoTHub> hubContext,
            IJsonSerializationService jsonSerializationService,
            ILogger<EventService> logger)
        {
            _hubContext = hubContext;
            _jsonSerializationService = jsonSerializationService;
            _logger = logger;
        }

        public async Task SendEvent(EventNameEnum eventName, object args)
        {
            var serializedData = _jsonSerializationService.Serialize(args);
            _logger.LogInformation("Event: {EventName}, Data: {Data}", eventName, serializedData);
            await _hubContext.Clients.All.SendAsync(eventName.ToString(), eventNamespace, serializedData);
        }

        public async Task SendEvent(EventNameEnum eventName, object args, CancellationToken cancellationToken)
        {
            var serializedData = _jsonSerializationService.Serialize(args);
            _logger.LogInformation("Event: {EventName}, Data: {Data}", eventName, serializedData);
            await _hubContext.Clients.All.SendAsync(eventName.ToString(), eventNamespace, serializedData, cancellationToken);
        }
    }
}