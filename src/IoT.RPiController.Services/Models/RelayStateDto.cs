using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class RelayStateDto : RelayBaseDto
    {
        public bool State { get; set; }
    }
}