using System.ComponentModel.DataAnnotations;
using IoT.RPiController.Data.Constants;

namespace IoT.RPiController.Services.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        [MinLength(3)]
        [MaxLength(200)]
        [Required(AllowEmptyStrings = false)]
        public string Login { get; set; } = string.Empty;

        [Required]
        [Range(
            TokenExpirationTime.MinTokenExpirationTime,
            TokenExpirationTime.MaxTokenExpirationTime
        )]
        public int AccessTokenExpirationTime { get; set; } = TokenExpirationTime.DefaultAccessTokenExpirationTime;

        [Required]
        [Range(
            TokenExpirationTime.MinTokenExpirationTime,
            TokenExpirationTime.MaxTokenExpirationTime
        )]
        public int RefreshTokenExpirationTime { get; set; } = TokenExpirationTime.DefaultRefreshTokenExpirationTime;
    }
}