using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class CreateModuleDto
    {
        [Required(AllowEmptyStrings = false)]
        public string moduleType { get; set; } = string.Empty;
    }
}
