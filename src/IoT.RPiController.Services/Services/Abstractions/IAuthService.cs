using System.Security.Claims;
using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions;

public interface IAuthService
{
    Task<AuthenticatedDto?> SignInAsync(AuthDto auth);
    Task<AuthenticatedDto?> RefreshTokenAsync(AuthenticatedDto authenticated);
    Task<bool> LogoutAsync(int userId);
}