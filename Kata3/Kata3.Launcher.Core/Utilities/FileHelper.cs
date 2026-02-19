namespace Kata3.Launcher.Core.Utilities;

public static class FileHelper
{
    public static void EnsureDirectoryExists(string path)
    {
        if (!string.IsNullOrEmpty(path))
            Directory.CreateDirectory(path);
    }
}