using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IoT.RPiController.Services.Options
{
    public class AuthConfigOptions
    {
        public const string AuthConfig = "AuthConfig";

        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Key { get; set; }
        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new(Encoding.UTF8.GetBytes(Key!));
    }
}
