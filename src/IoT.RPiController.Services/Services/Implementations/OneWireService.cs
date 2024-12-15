using IoT.RPiController.Services.Services.Abstractions;
using Iot.Device.OneWire;
using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Implementations;

public class OneWireService(
    IEventService eventService)
    : IOneWireService
{
    public async Task ReadOneWireAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }

        while(OneWireBus.EnumerateBusIds().Any())
        {
            var OneWireValues = new List<OneWireDto>();
            foreach (string busId in OneWireBus.EnumerateBusIds())
            {
                OneWireBus bus = new(busId);
                foreach (string devId in bus.EnumerateDeviceIds())
                {
                    
                    OneWireDevice dev = new(busId, devId);
                    if (OneWireThermometerDevice.IsCompatible(busId, devId))
                    {
                        OneWireThermometerDevice devTemp = new(busId, devId);
                        OneWireValues.Add( new OneWireDto{
                            Family = dev.Family,
                            DeviceId = dev.DeviceId,
                            BusId = dev.BusId,
                            Value = Math.Round(devTemp.ReadTemperatureAsync().Result.DegreesCelsius, 2)
                        });
                    }
                }
            }

            if (OneWireValues.Count > 0)
            {
                await eventService.SendEvent(EventNameEnum.OnOneWireStateUpdate, OneWireValues);
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(2000));
        }
    }
}