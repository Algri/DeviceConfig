using IoT.RPiController.Services.Enums;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions
{
    public interface IPowerBusService
    {
        PowerBusDto? ReadState(PowerBusEnum powerBusEnum);
        List<PowerBusDto> ReadAllStates();
        bool SwitchState(PowerBusEnum powerBusEnum, bool state);
    }
}
