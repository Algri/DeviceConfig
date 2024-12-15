using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IoT.RPiController.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Utilities")]
public class HealthCheckController(IHealthCheckService healthCheckService) : ControllerBase
{
    /// <summary>
    /// Get CPU temperature and Startup time of Linux device.
    /// </summary>
    [HttpGet("healthcheck")]
    public IActionResult HealthCheck()
    {
        var healthCheckResult = healthCheckService.PerformHealthCheck();

        return healthCheckResult is null ? BadRequest("Failed to perform health check") : Ok(healthCheckResult);
    }
}