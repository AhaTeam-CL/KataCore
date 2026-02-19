using System.Diagnostics;
using Kata3.Launcher.Core.Models.Launch;

namespace Kata3.Launcher.Core.Services.Launch;

public class ProcessLauncher
{
    public Process Start(string javaPath, List<string> jvmArgs, List<string> gameArgs, LaunchOptions options)
    {
        var arguments = string.Join(" ", jvmArgs.Concat(gameArgs));
        
        Console.WriteLine($"[DEBUG] Java 路径: {javaPath}");
        Console.WriteLine($"[DEBUG] 完整启动参数长度: {arguments.Length} 字符");
        Console.WriteLine($"[DEBUG] 启动参数预览 (前200字符): {arguments.Substring(0, Math.Min(200, arguments.Length))}");

        var psi = new ProcessStartInfo
        {
            FileName = javaPath,
            Arguments = arguments,
            WorkingDirectory = options.WorkingDirectory,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = options.RedirectOutput,
            RedirectStandardError = options.RedirectOutput
        };

        var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

        if (options.RedirectOutput)
        {
            process.OutputDataReceived += (s, e) => options.OutputHandler?.Invoke(e.Data);
            process.ErrorDataReceived += (s, e) => options.OutputHandler?.Invoke(e.Data);
        }

        process.Start();

        if (options.RedirectOutput)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        return process;
    }
}