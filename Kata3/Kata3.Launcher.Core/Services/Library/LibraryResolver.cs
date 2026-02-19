using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;
using MinecraftLibrary = Kata3.Launcher.Core.Models.Minecraft.Library; // 别名

namespace Kata3.Launcher.Core.Services.Library;

public class LibraryResolver
{
    private readonly OsHelper _osHelper;

    public LibraryResolver(OsHelper osHelper)
    {
        _osHelper = osHelper;
    }

    public List<Artifact> GetMissingLibraries(string minecraftRoot, VersionMeta version)
    {
        var missing = new List<Artifact>();

        foreach (var lib in version.Libraries) // lib 类型为 MinecraftLibrary
        {
            if (!IsLibraryApplicable(lib))
                continue;

            if (lib.Downloads?.Artifact != null)
            {
                string path = Path.Combine(minecraftRoot, "libraries", lib.Downloads.Artifact.Path);
                if (!File.Exists(path) || new FileInfo(path).Length != lib.Downloads.Artifact.Size)
                    missing.Add(lib.Downloads.Artifact);
            }

            if (lib.Natives != null)
            {
                string classifier = GetNativeClassifier(lib.Natives);
                if (!string.IsNullOrEmpty(classifier) && lib.Downloads?.Classifiers?.TryGetValue(classifier, out var nativeArtifact) == true)
                {
                    string path = Path.Combine(minecraftRoot, "libraries", nativeArtifact.Path);
                    if (!File.Exists(path) || new FileInfo(path).Length != nativeArtifact.Size)
                        missing.Add(nativeArtifact);
                }
            }
        }

        return missing;
    }

    private bool IsLibraryApplicable(MinecraftLibrary lib)
    {
        if (lib.Rules == null || lib.Rules.Count == 0)
            return true;

        bool allowed = false;
        foreach (var rule in lib.Rules)
        {
            if (!RuleSatisfied(rule))
                continue;

            if (rule.Action.Equals("allow", StringComparison.OrdinalIgnoreCase))
                allowed = true;
            else if (rule.Action.Equals("disallow", StringComparison.OrdinalIgnoreCase))
                allowed = false;
        }
        return allowed;
    }

    private bool RuleSatisfied(Rule rule)
    {
        if (rule.Os != null)
        {
            if (!_osHelper.Matches(rule.Os.Name, rule.Os.Version, rule.Os.Arch))
                return false;
        }
        return true;
    }

    private string? GetNativeClassifier(Dictionary<string, string> natives)
    {
        string osName = _osHelper.CurrentOs.ToString().ToLowerInvariant();
        return natives.TryGetValue(osName, out var classifier) ? classifier : null;
    }
}