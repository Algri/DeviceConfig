using System.ComponentModel.DataAnnotations;
using IoT.RPiController.Data.Enums;
using IoT.RPiController.Services.Enums;

namespace IoT.RPiController.Services.Models
{
    public class TimerValueDto
    {
        public int Id { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The relay number field must be more than 0.")]
        public int RelayNumber { get; set; } 
        
        [Required(AllowEmptyStrings = false)]
        [Range(1, int.MaxValue, ErrorMessage = "The timerSeconds field must be more than 0.")]
        public int Timer { get; set; } //TODO: rename to timer duration

        [Required(AllowEmptyStrings = false)]
        [Range(1, int.MaxValue, ErrorMessage = "The module id field must be more than 0.")]
        public int ModuleId { get; set; }
        
        public TimerActionEnum TimerAction { get; set; }
    }
}
