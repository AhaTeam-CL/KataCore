using System.Text.Json.Serialization;

namespace Kata3.Launcher.Core.Models.Minecraft;

public class Arguments
{
    [JsonPropertyName("jvm")]
    public List<object> Jvm { get; set; } = new();

    [JsonPropertyName("game")]
    public List<object> Game { get; set; } = new();
}