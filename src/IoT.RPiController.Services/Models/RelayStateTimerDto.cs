using System.ComponentModel.DataAnnotations;
using IoT.RPiController.Data.Enums;
using IoT.RPiController.Services.Enums;

namespace IoT.RPiController.Services.Models
{
    public class RelayStateTimerDto : RelayStateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "The timerSeconds field must be more than 0.")]
        public int TimerSeconds { get; set; }

        public TimerActionEnum TimerAction { get; set; }
    }
}