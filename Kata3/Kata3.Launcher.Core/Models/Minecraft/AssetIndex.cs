using System.Text.Json.Serialization;

namespace Kata3.Launcher.Core.Models.Minecraft;

public class AssetIndexRef
{
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("sha1")] public required string Sha1 { get; set; }
    [JsonPropertyName("size")] public int Size { get; set; }
    [JsonPropertyName("totalSize")] public int TotalSize { get; set; }
    [JsonPropertyName("url")] public required string Url { get; set; }
}

public class AssetIndex
{
    [JsonPropertyName("objects")]
    public Dictionary<string, AssetObject> Objects { get; set; } = new();
}

public class AssetObject
{
    [JsonPropertyName("hash")] public required string Hash { get; set; }
    [JsonPropertyName("size")] public long Size { get; set; }
}