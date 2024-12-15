using IoT.RPiController.Data.Auth;
using IoT.RPiController.Data.Constants;
using System.ComponentModel.DataAnnotations;

namespace IoT.RPiController.Data.Entities
{
    public class User
    {
        [Key] public int Id { get; set; }

        [MinLength(3)]
        [MaxLength(200)]
        [Required(AllowEmptyStrings = false)]
        public string Login { get; set; } = string.Empty;

        [MinLength(5)]
        [MaxLength(200)]
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Url)]
        public string? NodeRedUrl { get; set; } = string.Empty;
        
        public string RefreshToken { get; set; } = string.Empty;
        //TODO: we shall rename one of the similar props
        public DateTime RefreshTokenExpiryTime { get; set; }

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


        public async Task<string> GetPasswordHash() => await AuthHelper.GeneratePasswordHashAsync(Password);

        public bool ComparePasswordHash(string password) =>
            !string.Equals(password, Password);
    }
}