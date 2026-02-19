using System.Text.Json;
using Kata3.Launcher.Core.Models.Minecraft;

namespace Kata3.Launcher.Core.Services.Version;

public class VersionParser
{
    public async Task<VersionMeta> ParseAsync(string jsonPath)
    {
        await using var stream = File.OpenRead(jsonPath);
        return await JsonSerializer.DeserializeAsync<VersionMeta>(stream);
    }
}