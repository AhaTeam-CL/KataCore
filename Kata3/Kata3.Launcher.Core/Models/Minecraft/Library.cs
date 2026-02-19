using System.Text.Json.Serialization;

namespace Kata3.Launcher.Core.Models.Minecraft;

public class Library
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("downloads")] public Downloads? Downloads { get; set; }
    [JsonPropertyName("rules")] public List<Rule>? Rules { get; set; }
    [JsonPropertyName("natives")] public Dictionary<string, string>? Natives { get; set; }
    [JsonPropertyName("extract")] public ExtractRules? Extract { get; set; }
}

public class Downloads
{
    [JsonPropertyName("artifact")] public Artifact? Artifact { get; set; }
    [JsonPropertyName("classifiers")] public Dictionary<string, Artifact>? Classifiers { get; set; }
}

public class Artifact
{
    [JsonPropertyName("path")] public required string Path { get; set; }
    [JsonPropertyName("url")] public required string Url { get; set; }
    [JsonPropertyName("sha1")] public string? Sha1 { get; set; }
    [JsonPropertyName("size")] public int Size { get; set; }
}

public class ExtractRules
{
    [JsonPropertyName("exclude")] public List<string>? Exclude { get; set; }
}