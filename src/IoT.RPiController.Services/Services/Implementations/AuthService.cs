using IoT.RPiController.Data.Auth;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Options;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations
{
    public class AuthService(
        IOptions<AuthConfigOptions> config,
        IUserService userService,
        ILogger<AuthService> logger)
        : IAuthService
    {
        private readonly AuthConfigOptions _authConfig = config.Value;

        public async Task<AuthenticatedDto?> SignInAsync(AuthDto auth)
        {
            logger.LogInformation($"Signing in user: {auth.Login}");

            var user = await userService.GetByLoginAsync(auth.Login);
            if (user is null || AuthHelper.ComparePasswordsHash(auth.Password, user.Password) is false)
            {
                logger.LogWarning($"Sign in failed for user: {auth.Login}. Invalid credentials.");
                return null;
            }

            //Todo: ask Zhenya & Alex if we can get rid of Login here...
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Login),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var accessToken = GenerateAccessToken(claims, user.AccessTokenExpirationTime);

            var refreshToken = GenerateRefreshToken();

            await userService.UpdateRefreshTokenAsync(user.Id, refreshToken, user.RefreshTokenExpirationTime);

            logger.LogInformation($"User {auth.Login} signed in successfully.");
            return new AuthenticatedDto
            {
                Id = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthenticatedDto?> RefreshTokenAsync(AuthenticatedDto authenticated)
        {
            logger.LogInformation("Refreshing token.");

            // We are working with refresh token mainly
            var refreshToken = authenticated.RefreshToken;
            var accessToken = authenticated.AccessToken;

            // We validate the access token without checking its expiration
            var claimsFromToken = ExtractClaimsFromToken(accessToken);
            if (claimsFromToken is null)
            {
                logger.LogWarning("Invalid access token, unable to refresh.");
                return null;
            }

            // Getting the User.ID from token
            var userIdClaim = claimsFromToken.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                logger.LogWarning("User Id not found in token.");
                return null;
            }

            var user = await userService.GetByIdAsync(Convert.ToInt32(userIdClaim));
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                logger.LogWarning($"Refresh token invalid or expired for user with ID {userIdClaim}.");
                return null;
            }

            // Generate new tokens
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Login), //TODO: remove if FE and Mobile are not using it
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var newAccessToken = GenerateAccessToken(claims, user.AccessTokenExpirationTime);
            var newRefreshToken = GenerateRefreshToken();

            // Update the user's refresh token and expiration
            await userService.UpdateRefreshTokenAsync(user.Id, newRefreshToken, user.RefreshTokenExpirationTime);

            logger.LogInformation("Token refreshed successfully.");

            return new AuthenticatedDto
            {
                Id = user.Id,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            logger.LogInformation($"Logging out user with ID: {userId}");

            var user = await userService.GetByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning($"Logout failed. User with ID {userId} not found.");
                return false;
            }

            var tokenReset = await userService.ResetRefreshTokenAsync(user.Id);

            if (tokenReset)
            {
                logger.LogInformation($"User with ID {userId} logged out successfully.");
            }
            else
            {
                logger.LogWarning($"User with ID {userId} wasn't found, log out failed.");
            }

            return tokenReset;
        }

        #region helpers

        private string GenerateAccessToken(IEnumerable<Claim> claims, int tokenExpirationTime)
        {
            logger.LogInformation("Generating access token.");

            var token = new JwtSecurityToken(
                issuer: _authConfig.Issuer,
                audience: _authConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(tokenExpirationTime)),
                signingCredentials: new SigningCredentials(_authConfig.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            logger.LogInformation("Access token generated successfully.");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            logger.LogInformation("Generating refresh token.");

            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            logger.LogInformation("Refresh token generated successfully.");
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? ExtractClaimsFromToken(string token)
        {
            logger.LogInformation("Getting principal from expired token.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = false, // Ignore token expiration! We don't need to check access token exp.date
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _authConfig.Issuer,
                ValidAudience = _authConfig.Audience,
                IssuerSigningKey = _authConfig.GetSymmetricSecurityKey(),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                logger.LogInformation("Principal retrieved successfully.");
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Principal retrieving error.");
                return null;
            }
        }

        #endregion
    }
}