using Iot.Device.Mcp23xxx;
using IoT.RPiController.Services.Enums;

namespace IoT.RPiController.Services.Models
{
    public class Mcp23008Dto
    {
        public int Id { get; set; }
        public byte Address { get; set; }
        public required Mcp23008 Driver { get; set; }
        public DeviceType Type { get; set; }
    }
}