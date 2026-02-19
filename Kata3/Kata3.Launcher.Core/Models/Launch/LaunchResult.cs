using System.Diagnostics;

namespace Kata3.Launcher.Core.Models.Launch;

public class LaunchResult
{
    public bool Success { get; set; }
    public Process? Process { get; set; }
    public string? Error { get; set; }
}