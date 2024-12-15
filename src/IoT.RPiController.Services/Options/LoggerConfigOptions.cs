namespace IoT.RPiController.Services.Options;

public class LoggerConfigOptions
{
    public const string LoggerConfig = "Logger";

    public string? Path { get; set; }
    public string? LokiServiceKey { get; set; }
    public string? LokiServiceValue { get; set; }
}