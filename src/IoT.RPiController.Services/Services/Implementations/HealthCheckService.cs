using System.Runtime.InteropServices;
using Iot.Device.CpuTemperature;
using IoT.RPiController.Services.Models;
using IoT.RPiController.Services.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace IoT.RPiController.Services.Services.Implementations;

public class HealthCheckService(ILogger<HealthCheckService> logger) : IHealthCheckService
{
    /// <summary>
    /// Performs a health check on the system.
    /// </summary>
    /// <returns>
    /// A HealthCheckDto containing the results of the health check, or null if the check fails.
    /// </returns>
    public HealthCheckDto? PerformHealthCheck()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        var startupTimeInSeconds = uptime.TotalSeconds;

        if (!TryGetCpuTemperature(out var cpuAverageTemperature, out var cpuMaxTemperature))
        {
            logger.LogError("Failed to read CPU temperature.");
            return null;
        }

        var healthCheckResult = new HealthCheckDto
        {
            CpuAverageTemperature = cpuAverageTemperature,
            CpuMaxTemperature = cpuMaxTemperature,
            StartupTimeInSeconds = startupTimeInSeconds
        };

        return healthCheckResult;
    }

    /// <summary>
    /// Attempts to get the CPU temperature of Linux device.
    /// </summary>
    /// <param name="cpuAverageTemperature">The average CPU temperature, if available.</param>
    /// <param name="cpuMaxTemperature">The maximum CPU temperature, if available.</param>
    /// <returns>
    /// True if the CPU temperature was successfully read; otherwise, false.
    /// </returns>
    private bool TryGetCpuTemperature(out double cpuAverageTemperature, out double cpuMaxTemperature)
    {
        cpuAverageTemperature = 0.0;
        cpuMaxTemperature = 0.0;

        try
        {
            // Mocked data for non-relevant OS
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cpuAverageTemperature = 80085.0;
                cpuMaxTemperature = 0.112358;
                return true;
            }

            // For Linux and macOS, use the Iot.Device.CpuTemperature library
            var cpuTemperature = new CpuTemperature();

            if (!cpuTemperature.IsAvailable)
            {
                return false;
            }

            var cpuTemperatures = cpuTemperature.ReadTemperatures();
            var validReadings = cpuTemperatures
                .Where(entry => !double.IsNaN(entry.Temperature.DegreesCelsius))
                .Select(entry => entry.Temperature.DegreesCelsius)
                .ToList();

            if (validReadings.Count == 0)
            {
                return false;
            }

            cpuAverageTemperature = validReadings.Average();
            cpuMaxTemperature = validReadings.Max();

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get CPU temperature.");

            return false;
        }
    }
}