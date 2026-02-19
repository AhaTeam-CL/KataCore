using System.Text.Json.Serialization;

namespace Kata3.Launcher.Core.Models.Minecraft;

public class VersionMeta
{
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("mainClass")] public required string MainClass { get; set; }
    [JsonPropertyName("libraries")] public List<Library> Libraries { get; set; } = new();
    [JsonPropertyName("arguments")] public Arguments? Arguments { get; set; }
    [JsonPropertyName("minecraftArguments")] public string? LegacyArguments { get; set; }
    [JsonPropertyName("assetIndex")] public AssetIndexRef? AssetIndex { get; set; }
    [JsonPropertyName("assets")] public string? AssetsVersion { get; set; }
    [JsonPropertyName("releaseTime")] public DateTime ReleaseTime { get; set; }
}