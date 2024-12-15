using System.ComponentModel.DataAnnotations;
using Iot.Device.OneWire;

namespace IoT.RPiController.Services.Models
{
    public class OneWireDto
    {
        public DeviceFamily Family { get; set; } = new DeviceFamily();
        public string DeviceId { get; set; } = string.Empty;
        public string BusId { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}
