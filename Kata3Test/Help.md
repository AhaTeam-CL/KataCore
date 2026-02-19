# 使用KataCore开发我的世界启动器

---

## 依赖库

**Kata库**：必须添加的，其中包含Kata所有的方法和依赖，如需下载依赖库点击[这里]()下载dll文件。

添加引用并调用，如下：

```csharp
using Kata3.Launcher.Core;
using Kata3.Launcher.Core.Models.Auth;
using Kata3.Launcher.Core.Models.Launch;
using Kata3.Launcher.Core.Services.Asset;

// 可选
using Kata3.Launcher.Core.Utilities;
```

---

## 创建玩家配置（离线）

```csharp
var account = new Account
{
    UserName = "玩家名称",
    UserType = "msa",
    
    // 可选
    Uuid = Guid.Parse("00000000-ffff-ffff-ffff-ffffffff4de2"),
    AccessToken = "00000FFFFFFFFFFFFFFFFFFFFFF4DE2A"
};
```

---

## 启动设置

```csharp
var options = new LaunchOptions
{
    MinecraftRoot = Path.GetFullPath(@".minecraft"), // 使用绝对路径
    VersionId = "1.19.2",
    WorkingDirectory = Path.GetFullPath(@".minecraft"), // 统一使用项目根目录的.minecraft
    JavaPath = "D:\\Program Files\\JAVA\\bin\\java.exe",
    Account = account,
    MaxMemory = 4198,
    Width = 854,
    Height = 480,
    UseVersionIsolation = true,
    OutputHandler = msg => Console.WriteLine($"[GAME] {msg}")
};
```

**类结构**:

```csharp
namespace Kata3.Launcher.Core.Models.Launch;

public class LaunchOptions
{
    public required string MinecraftRoot { get; set; }
    public required string VersionId { get; set; }
    public required string WorkingDirectory { get; set; }
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
```

---

## 下载器（检查资源）

```csharp
var progress = new Progress<AssetDownloadProgress>(p =>
{
    Console.WriteLine($"下载资源: {p.Completed}/{p.Total} ({p.Percentage:F1}%)");
});
```

---

## 启动!

```csharp
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
```