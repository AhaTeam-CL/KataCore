using System.Text.Json;

namespace Kata3.Launcher.Core.Utilities;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);
    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);
}