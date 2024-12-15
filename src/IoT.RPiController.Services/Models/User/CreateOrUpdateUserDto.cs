using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Services.Models
{
    public class CreateOrUpdateUserDto : UserDto, IValidatableObject
    {
        [MinLength(5)]
        [MaxLength(200)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Password) && Password.Length < 5)
            {
                yield return new ValidationResult("The Password must be at least 5 characters long.", new[] { nameof(Password) });
            }
        }
    }
}
