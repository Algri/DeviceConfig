using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class AuthDto
    {
        [MinLength(3)]
        [MaxLength(200)]
        [Required(AllowEmptyStrings = false)]
        public string Login { get; set; } = string.Empty;

        [MinLength(5)]
        [MaxLength(200)]
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
