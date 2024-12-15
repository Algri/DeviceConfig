using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class PowerBusDto
    {
        [Required(AllowEmptyStrings = false)]
        public string BusName { get; set; } = string.Empty;
        public bool State { get; set; }
    }
}
