using System.IO.Compression;
using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Library;

public class NativeExtractor
{
    private readonly OsHelper _osHelper;

    public NativeExtractor(OsHelper osHelper)
    {
        _osHelper = osHelper;
    }

    public async Task<string> ExtractAsync(string minecraftRoot, string versionId, VersionMeta version, bool useIsolation = true)
    {
        string baseDir = useIsolation
            ? Path.Combine(minecraftRoot, "versions", versionId, "natives")
            : Path.Combine(Path.GetTempPath(), "kata3_natives", versionId);

        Directory.CreateDirectory(baseDir);

        var nativeLibs = version.Libraries.Where(l => l.Natives != null && l.Natives.Count > 0);
        foreach (var lib in nativeLibs)
        {
            string classifier = GetNativeClassifier(lib.Natives);
            if (string.IsNullOrEmpty(classifier)) continue;

            if (lib.Downloads?.Classifiers?.TryGetValue(classifier, out var artifact) != true)
                continue;

            string jarPath = Path.Combine(minecraftRoot, "libraries", artifact.Path);
            if (!File.Exists(jarPath)) continue;

            await ExtractNativeFiles(jarPath, baseDir, lib.Extract?.Exclude);
        }

        return baseDir;
    }

    private string GetNativeClassifier(Dictionary<string, string> natives)
    {
        string osName = _osHelper.CurrentOs.ToString().ToLowerInvariant();
        return natives.TryGetValue(osName, out var classifier) ? classifier : null;
    }

    private async Task ExtractNativeFiles(string jarPath, string targetDir, List<string> excludePatterns)
    {
        using var archive = ZipFile.OpenRead(jarPath);
        foreach (var entry in archive.Entries)
        {
            if (ShouldExtract(entry, excludePatterns))
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(entry.Name));
                if (!File.Exists(destFile))
                {
                    entry.ExtractToFile(destFile, overwrite: true);
                }
            }
        }
        await Task.CompletedTask;
    }

    private bool ShouldExtract(ZipArchiveEntry entry, List<string> excludePatterns)
    {
        if (string.IsNullOrEmpty(entry.Name)) return false;
        string ext = Path.GetExtension(entry.Name).ToLowerInvariant();
        if (ext != ".dll" && ext != ".so" && ext != ".dylib") return false;

        if (excludePatterns != null)
        {
            foreach (var pattern in excludePatterns)
            {
                if (entry.Name.Contains(pattern)) return false;
            }
        }
        return true;
    }
}