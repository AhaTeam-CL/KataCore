using System.Runtime.InteropServices;

namespace Kata3.Launcher.Core.Utilities;

public class OsHelper
{
    public PlatformID CurrentOs { get; }

    public OsHelper()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            CurrentOs = PlatformID.Win32NT;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            CurrentOs = PlatformID.Unix;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            CurrentOs = PlatformID.MacOSX;
        else
            CurrentOs = PlatformID.Other;
    }

    public bool Matches(string? osName, string? osVersion = null, string? osArch = null)
    {
        if (string.IsNullOrEmpty(osName))
            return true;

        bool nameMatch = osName.ToLowerInvariant() switch
        {
            "windows" => CurrentOs == PlatformID.Win32NT,
            "linux" => CurrentOs == PlatformID.Unix,
            "osx" => CurrentOs == PlatformID.MacOSX,
            _ => false
        };

        if (!nameMatch) return false;

        // 版本和架构检查可扩展（目前忽略）
        return true;
    }
}