using System.Text.Json.Serialization;

namespace Kata3.Launcher.Core.Models.Minecraft;

public class Rule
{
    [JsonPropertyName("action")] public required string Action { get; set; }
    [JsonPropertyName("os")] public OsRule? Os { get; set; }
    [JsonPropertyName("features")] public Dictionary<string, bool>? Features { get; set; }
}

public class OsRule
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("version")] public string? Version { get; set; }
    [JsonPropertyName("arch")] public string? Arch { get; set; }
}