using System.Text;
using Kata3.Launcher.Core.Models.Launch;
using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Launch;

public class ArgumentBuilder
{
    private readonly OsHelper _osHelper;

    public ArgumentBuilder(OsHelper osHelper)
    {
        _osHelper = osHelper;
    }

    public (List<string> jvmArgs, List<string> gameArgs) Build(
        LaunchOptions options,
        VersionMeta version,
        string classPath,
        string nativesDir)
    {
        var jvm = new List<string>();
        var game = new List<string>();

        // 编码相关
        jvm.Add("-Dstderr.encoding=UTF-8");
        jvm.Add("-Dstdout.encoding=UTF-8");
        jvm.Add("-Dfile.encoding=UTF-8");

        // GC 优化
        jvm.Add("-XX:+UseG1GC");
        jvm.Add("-XX:-UseAdaptiveSizePolicy");
        jvm.Add("-XX:-OmitStackTraceInFastThrow");

        // 安全与兼容
        jvm.Add("-Djdk.lang.Process.allowAmbiguousCommands=true");
        jvm.Add("-Dfml.ignoreInvalidMinecraftCertificates=True");
        jvm.Add("-Dfml.ignorePatchDiscrepancies=True");
        jvm.Add("-Dlog4j2.formatMsgNoLookups=true");

        // 启动器标识
        jvm.Add("-Dminecraft.launcher.brand=Kata3");
        jvm.Add("-Dminecraft.launcher.version=1.0.0");

        if (_osHelper.CurrentOs == PlatformID.Win32NT)
        {
            jvm.Add("-Dos.name=\"Windows 10\"");
            jvm.Add("-Dos.version=10.0");
        }

        jvm.Add($"-Djava.library.path=\"{nativesDir}\"");
        jvm.Add("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump");

        jvm.Add($"-Xmn{options.MinMemory}m");
        jvm.Add($"-Xmx{options.MaxMemory}m");

        jvm.AddRange(options.JvmArguments);
        jvm.Add($"-cp \"{classPath}\"");
        jvm.Add(version.MainClass);
        
        Console.WriteLine($"[DEBUG] JVM 参数数量: {jvm.Count}");
        Console.WriteLine($"[DEBUG] 最后几个 JVM 参数:");
        for (int i = Math.Max(0, jvm.Count - 3); i < jvm.Count; i++)
        {
            Console.WriteLine($"  [{i}] {jvm[i]}");
        }

        // 游戏参数
        Console.WriteLine($"[DEBUG] version.Arguments 是否为 null: {version.Arguments == null}");
        if (version.Arguments?.Game != null)
        {
            Console.WriteLine($"[DEBUG] 使用 Arguments.Game，参数数量: {version.Arguments.Game.Count}");
            int processedCount = 0;
            foreach (var obj in version.Arguments.Game)
            {
                Console.WriteLine($"[DEBUG] 处理参数对象类型: {obj.GetType().Name}");
                
                // 处理字符串参数
                if (obj is string str)
                {
                    Console.WriteLine($"[DEBUG] 原始字符串参数: {str}");
                    string replaced = ReplacePlaceholders(str, options, version, nativesDir);
                    Console.WriteLine($"[DEBUG] 替换后参数: {replaced}");
                    game.Add(replaced);
                    processedCount++;
                }
                // 处理 JSON 元素（可能是规则对象或其他复杂结构）
                else if (obj is System.Text.Json.JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        string strValue = jsonElement.GetString() ?? "";
                        Console.WriteLine($"[DEBUG] JSON 字符串参数: {strValue}");
                        string replaced = ReplacePlaceholders(strValue, options, version, nativesDir);
                        Console.WriteLine($"[DEBUG] 替换后参数: {replaced}");
                        game.Add(replaced);
                        processedCount++;
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] JSON 非字符串值，跳过: {jsonElement.ValueKind}");
                    }
                }
                else
                {
                    Console.WriteLine($"[DEBUG] 未知参数类型，跳过");
                }
            }
            Console.WriteLine($"[DEBUG] 实际处理的字符串参数数量: {processedCount}");
        }
        else if (!string.IsNullOrEmpty(version.LegacyArguments))
        {
            Console.WriteLine($"[DEBUG] 使用 LegacyArguments");
            string replaced = ReplacePlaceholders(version.LegacyArguments, options, version, nativesDir);
            game.AddRange(replaced.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        else
        {
            Console.WriteLine($"[DEBUG] 使用默认参数");
            // 默认参数
            game.Add($"--username {options.Account?.UserName ?? "Player"}");
            game.Add($"--version {version.Id}");
            game.Add($"--gameDir \"{Path.Combine(options.MinecraftRoot, "versions", version.Id)}\"");
            game.Add($"--assetsDir \"{Path.Combine(options.MinecraftRoot, "assets")}\"");
            game.Add($"--assetIndex {version.AssetIndex?.Id ?? version.AssetsVersion ?? version.Id}");
            game.Add($"--uuid {options.Account?.Uuid.ToString()?.Replace("-", "") ?? Guid.NewGuid().ToString().Replace("-", "")}");
            game.Add($"--accessToken {options.Account?.AccessToken ?? "0"}");
            game.Add($"--clientId {options.Account?.ClientId ?? "0"}");
            game.Add($"--xuid {options.Account?.Xuid ?? "0"}");
            game.Add($"--userType {options.Account?.UserType ?? "msa"}");
            game.Add($"--versionType Kata3");
            game.Add($"--width {options.Width}");
            game.Add($"--height {options.Height}");
            if (options.Fullscreen)
                game.Add("--fullscreen");
        }
                
        foreach (var kv in options.GameArguments)
            game.Add($"{kv.Key}={kv.Value}");
                    
        Console.WriteLine($"[DEBUG] 游戏参数数量: {game.Count}");
        Console.WriteLine($"[DEBUG] 游戏参数:");
        foreach (var arg in game)
        {
            Console.WriteLine($"  {arg}");
        }

        return (jvm, game);
    }

    private string ReplacePlaceholders(string input, LaunchOptions options, VersionMeta version, string nativesDir)
    {
        return input
            .Replace("${auth_player_name}", options.Account?.UserName ?? "Player")
            .Replace("${version_name}", version.Id)
            .Replace("${game_directory}", Path.Combine(options.MinecraftRoot, "versions", version.Id))
            .Replace("${assets_root}", Path.Combine(options.MinecraftRoot, "assets"))
            .Replace("${assets_index_name}", version.AssetIndex?.Id ?? version.AssetsVersion ?? version.Id)
            .Replace("${auth_uuid}", options.Account?.Uuid.ToString()?.Replace("-", "") ?? "")
            .Replace("${auth_access_token}", options.Account?.AccessToken ?? "")
            .Replace("${clientid}", options.Account?.ClientId ?? "0")
            .Replace("${auth_xuid}", options.Account?.Xuid ?? "0")
            .Replace("${user_type}", options.Account?.UserType ?? "msa")
            .Replace("${version_type}", "Kata3")
            .Replace("${screen_width}", options.Width.ToString())
            .Replace("${screen_height}", options.Height.ToString())
            .Replace("${natives_directory}", nativesDir)
            .Replace("${classpath}", "${classpath}")
            .Replace("${launcher_name}", "Kata3")
            .Replace("${launcher_version}", "1.0.0");
    }
}