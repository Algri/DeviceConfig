using IoT.RPiController.Services.Models;

namespace IoT.RPiController.Services.Services.Abstractions;

public interface IHealthCheckService
{
    HealthCheckDto? PerformHealthCheck();
}