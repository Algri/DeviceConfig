using System.Security.Claims;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoT.RPiController.WebApi.Controllers;

[ApiExplorerSettings(GroupName = "Auth")]
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Post([FromBody] AuthDto? loginRequest)
    {
        if (loginRequest is null)
            return BadRequest("Invalid client request");

        var authenticatedResponse = await authService.SignInAsync(loginRequest);
        if (authenticatedResponse is null)
        {
            return Unauthorized();
        }

        return Ok(authenticatedResponse);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] AuthenticatedDto? refreshTokenRequest)
    {
        if (refreshTokenRequest is null)
            return BadRequest("Invalid client request");

        var refreshedTokenResponse = await authService.RefreshTokenAsync(refreshTokenRequest);
        if (refreshedTokenResponse is null)
        {
            return Unauthorized("Sign in failed. Invalid credentials.");
        }

        return Ok(refreshedTokenResponse);
    }

    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout()
    {
        // Retrieve the user ID from the claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("No user ID found."); // If user ID is not found, return Unauthorized
        }

        // Convert the user ID to integer (assuming it's stored as an integer)
        var userId = Convert.ToInt32(userIdClaim);
        
        var logoutResult = await authService.LogoutAsync(userId);
        if (!logoutResult)
        {
            return BadRequest("Logout failed. Check the logs.");
        }

        return Ok("Logout successful.");
    }

    /// <summary>
    /// Check if your token is valid.
    /// </summary>
    [HttpGet("validate"), Authorize]
    public IActionResult TokenValidate()
    {
        return Ok("Token is valid, go on with destructing the world!");
    }
}