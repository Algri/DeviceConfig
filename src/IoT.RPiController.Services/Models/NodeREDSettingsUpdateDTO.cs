using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class NodeREDSettingsUpdateDTO
    {
        [Required(AllowEmptyStrings = false)]
        public string url { get; set; } = string.Empty;
    }
}
