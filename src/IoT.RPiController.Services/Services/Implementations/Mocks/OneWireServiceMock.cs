using IoT.RPiController.Services.Services.Abstractions;
using Iot.Device.OneWire;
using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Implementations;

public class OneWireServiceMock(
    IEventService eventService)
    : IOneWireService
{
    public async Task ReadOneWireAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        while (true)
        {
            var OneWireValues = new List<OneWireDto>
            {
                new OneWireDto
                {
                    Family = 0,
                    DeviceId = "fake-deviceId",
                    BusId = "fake-busId",
                    Value = Math.Round(new Random().NextDouble() * 20 + 25, 2)
                }
            };

            await eventService.SendEvent(EventNameEnum.OnOneWireStateUpdate, OneWireValues);
            Thread.Sleep(TimeSpan.FromMilliseconds(2000));
        }
    }
}