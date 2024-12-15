using System.Text.Json;
using System.Text.Json.Serialization;
using IoT.RPiController.Services.Services.Abstractions;

namespace IoT.RPiController.Services.Services.Implementations;

public class JsonSerializationService : IJsonSerializationService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string Serialize(object? value)
    {
        return JsonSerializer.Serialize(value, _jsonSerializerOptions);
    }

    public string SerializePlain(object? value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
    }

    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}