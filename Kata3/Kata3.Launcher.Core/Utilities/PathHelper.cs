namespace Kata3.Launcher.Core.Utilities;

public static class PathHelper
{
    public static string GetNativeDirectory(string minecraftRoot, string versionId, bool isolated)
    {
        return isolated
            ? Path.Combine(minecraftRoot, "versions", versionId, "natives")
            : Path.Combine(Path.GetTempPath(), "kata3_natives", versionId);
    }
}