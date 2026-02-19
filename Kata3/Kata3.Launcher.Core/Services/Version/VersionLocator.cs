using Kata3.Launcher.Core.Models.Minecraft;

namespace Kata3.Launcher.Core.Services.Version;

public class VersionLocator
{
    private readonly VersionParser _parser;

    public VersionLocator(VersionParser parser)
    {
        _parser = parser;
    }

    public async Task<List<VersionMeta>> GetLocalVersionsAsync(string minecraftRoot)
    {
        var versionsDir = Path.Combine(minecraftRoot, "versions");
        if (!Directory.Exists(versionsDir))
            return new List<VersionMeta>();

        var versions = new List<VersionMeta>();
        foreach (var dir in Directory.GetDirectories(versionsDir))
        {
            var versionId = Path.GetFileName(dir);
            var jsonPath = Path.Combine(dir, $"{versionId}.json");
            if (File.Exists(jsonPath))
            {
                try
                {
                    var version = await _parser.ParseAsync(jsonPath);
                    versions.Add(version);
                }
                catch { /* 忽略损坏的版本文件 */ }
            }
        }
        return versions;
    }
}