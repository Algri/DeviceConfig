using IoT.RPiController.Data.Entities;
using Newtonsoft.Json;

namespace IoT.RPiController.Services.Helpers;

public static class CommonHelper
{
    public static string GetTypeFullName<T>() => typeof(T).FullName ?? throw new ArgumentException("Incorrect type is specified.");

    public static T GetTypeFullName<T>(GeneralConfiguration config)
        => JsonConvert.DeserializeObject<T>(config.Value) ?? throw new ArgumentException("Incorrect type is specified.");
}