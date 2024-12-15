namespace IoT.RPiController.Services.Models;

public class HealthCheckDto
{
    public double CpuAverageTemperature { get; set; }
    public double CpuMaxTemperature { get; set; }
    public double StartupTimeInSeconds { get; set; }
}