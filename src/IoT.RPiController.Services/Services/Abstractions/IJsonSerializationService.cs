namespace IoT.RPiController.Services.Services.Abstractions;

public interface IJsonSerializationService
{
    string Serialize(object? value);
    string SerializePlain(object? value);
    T? Deserialize<T>(string json);
}