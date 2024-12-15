using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class ModuleDto
    {
        [Required(AllowEmptyStrings = false)]
        public string ModuleType { get; set; } = string.Empty;

        [StringLength(25)]
        public string? ModuleName { get; set; } = null;
    }
}
