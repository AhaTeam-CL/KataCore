using Kata3.Launcher.Core.Utilities;
using MinecraftLibrary = Kata3.Launcher.Core.Models.Minecraft.Library; // 别名

namespace Kata3.Launcher.Core.Services.Launch;

public class ClassPathBuilder
{
    private readonly OsHelper _osHelper;

    public ClassPathBuilder(OsHelper osHelper)
    {
        _osHelper = osHelper;
    }

    public string Build(string minecraftRoot, Models.Minecraft.VersionMeta version)
    {
        var paths = new List<string>();

        foreach (var lib in version.Libraries) // lib 类型为 MinecraftLibrary
        {
            if (!IsLibraryApplicable(lib))
                continue;

            if (lib.Downloads?.Artifact != null)
            {
                string path = Path.Combine(minecraftRoot, "libraries", lib.Downloads.Artifact.Path);
                paths.Add(path);
            }
        }

        string versionJar = Path.Combine(minecraftRoot, "versions", version.Id, $"{version.Id}.jar");
        if (File.Exists(versionJar))
        {
            paths.Add(versionJar);
            Console.WriteLine($"[DEBUG] 添加客户端 JAR 到类路径: {versionJar}");
        }
        else
        {
            Console.WriteLine($"[DEBUG] 客户端 JAR 不存在: {versionJar}");
        }

        string classPath = string.Join(Path.PathSeparator, paths);
        // 标准化路径分隔符
        classPath = classPath.Replace('/', Path.DirectorySeparatorChar);
        Console.WriteLine($"[DEBUG] 完整类路径长度: {classPath.Length} 字符");
        Console.WriteLine($"[DEBUG] 路径分隔符: '{Path.PathSeparator}'");
        return classPath;
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

    private bool RuleSatisfied(Models.Minecraft.Rule rule)
    {
        if (rule.Os != null)
        {
            if (!_osHelper.Matches(rule.Os.Name, rule.Os.Version, rule.Os.Arch))
                return false;
        }
        return true;
    }
}