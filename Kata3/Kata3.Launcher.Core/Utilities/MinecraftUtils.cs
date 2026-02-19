using System.Runtime.InteropServices;

namespace Kata3.Launcher.Core.Utilities;

public static class MinecraftUtils
{
    public static List<string> GetJavaList(string minecraftRoot)
    {
        var versions = new List<string>();
        if (string.IsNullOrEmpty(minecraftRoot) || !Directory.Exists(minecraftRoot))
            return versions;

        string versionsDir = Path.Combine(minecraftRoot, "versions");
        if (!Directory.Exists(versionsDir))
            return versions;

        foreach (string dir in Directory.GetDirectories(versionsDir))
        {
            string versionId = Path.GetFileName(dir);
            string jsonPath = Path.Combine(dir, $"{versionId}.json");
            if (File.Exists(jsonPath))
            {
                versions.Add(versionId);
            }
        }

        return versions;
    }

    private static string GetJavaExecutable(string javaHome)
    {
        string exeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "java.exe" : "java";
        return Path.Combine(javaHome, "bin", exeName);
    }

    /// <summary>
    /// 根据 Minecraft 根目录获取可用的版本 ID 列表
    /// </summary>
    /// <param name="minecraftRoot">.minecraft 目录的路径</param>
    /// <returns>版本 ID 列表（例如 ["1.19.2", "1.20.4"]）</returns>
    public static List<string> GetVersionList(string minecraftRoot)
    {
        var versions = new List<string>();
        if (string.IsNullOrEmpty(minecraftRoot) || !Directory.Exists(minecraftRoot))
            return versions;

        string versionsDir = Path.Combine(minecraftRoot, "versions");
        if (!Directory.Exists(versionsDir))
            return versions;

        foreach (string dir in Directory.GetDirectories(versionsDir))
        {
            string versionId = Path.GetFileName(dir);
            string jsonPath = Path.Combine(dir, $"{versionId}.json");
            if (File.Exists(jsonPath))
            {
                versions.Add(versionId);
            }
        }

        return versions;
    }
}