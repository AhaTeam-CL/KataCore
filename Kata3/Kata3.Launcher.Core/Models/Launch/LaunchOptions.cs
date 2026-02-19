using Kata3.Launcher.Core.Models.Auth;

namespace Kata3.Launcher.Core.Models.Launch;

public class LaunchOptions
{
    private string _minecraftRoot;
    public required string MinecraftRoot
    {
        get => _minecraftRoot;
        set => _minecraftRoot = Path.GetFullPath(value);
    }

    private string _workingDirectory;
    public required string WorkingDirectory
    {
        get => _workingDirectory;
        set => _workingDirectory = Path.GetFullPath(value);
    }

    public required string VersionId { get; set; }
    public string JavaPath { get; set; } = "java";
    public int MinMemory { get; set; } = 512;
    public int MaxMemory { get; set; } = 2048;
    public List<string> JvmArguments { get; set; } = new();
    public Account? Account { get; set; }
    public int Width { get; set; } = 854;
    public int Height { get; set; } = 480;
    public bool Fullscreen { get; set; } = false;
    public Dictionary<string, string> GameArguments { get; set; } = new();
    public bool RedirectOutput { get; set; } = true;
    public Action<string>? OutputHandler { get; set; }
    public bool UseVersionIsolation { get; set; } = true;
}