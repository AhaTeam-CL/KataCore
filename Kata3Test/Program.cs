using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Kata3.Launcher.Core;
using Kata3.Launcher.Core.Models.Auth;
using Kata3.Launcher.Core.Models.Launch;
using Kata3.Launcher.Core.Services.Asset;
using Kata3.Launcher.Core.Utilities;

// 创建账户
var account = new Account()
{
    UserName = "Ikun"
};

// 配置启动参数
var options = new LaunchOptions()
{
    MinecraftRoot = @".minecraft",
    WorkingDirectory = @".minecraft", // 统一使用目录
    VersionId = "1.19.2",
    JavaPath = @"D:\Program Files\JAVA\bin\java.exe",
    MaxMemory = 5000,
    Account = account,
    OutputHandler = msg => Console.WriteLine($"[GAME] {msg}")
};

// 检查
var progress = new Progress<AssetDownloadProgress>(p =>
{
    Console.WriteLine($"下载资源: {p.Completed}/{p.Total} ({p.Percentage:F1}%)");
});

var launcher = new MinecraftLauncher();
var result = await launcher.LaunchAsync(options, progress);

if (result.Success)
{
    await result.Process.WaitForExitAsync();
    Console.WriteLine("游戏已退出。");
}
else
{
    Console.WriteLine($"启动失败: {result.Error}");
}

