using System.Security.Cryptography;
using System.Text.Json;
using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Version;

public class VersionManager
{
    private readonly HttpDownloader _httpDownloader;
    private readonly VersionParser _versionParser;

    public VersionManager(HttpDownloader httpDownloader, VersionParser versionParser)
    {
        _httpDownloader = httpDownloader;
        _versionParser = versionParser;
    }

    /// <summary>
    /// 确保指定版本的游戏文件完整可用
    /// </summary>
    /// <param name="minecraftRoot">Minecraft 根目录</param>
    /// <param name="versionId">版本 ID</param>
    /// <returns>版本元数据</returns>
    public async Task<VersionMeta> EnsureVersionIntegrityAsync(string minecraftRoot, string versionId)
    {
        string versionDir = Path.Combine(minecraftRoot, "versions", versionId);
        string versionJsonPath = Path.Combine(versionDir, $"{versionId}.json");
        string versionJarPath = Path.Combine(versionDir, $"{versionId}.jar");

        // 确保版本目录存在
        Directory.CreateDirectory(versionDir);

        // 检查并下载版本配置文件
        if (!File.Exists(versionJsonPath))
        {
            Console.WriteLine($"正在下载 Minecraft {versionId} 版本信息...");
            await DownloadVersionManifestAsync(versionId, versionJsonPath);
            Console.WriteLine("版本信息下载完成");
        }

        // 检查并下载客户端 JAR 文件
        if (!File.Exists(versionJarPath))
        {
            Console.WriteLine($"正在下载 Minecraft {versionId} 客户端...");
            await DownloadClientJarAsync(versionJsonPath, versionJarPath);
            Console.WriteLine("客户端下载完成");
        }

        // 解析并返回版本元数据
        return await _versionParser.ParseAsync(versionJsonPath);
    }

    /// <summary>
    /// 下载版本清单并获取指定版本的配置
    /// </summary>
    private async Task DownloadVersionManifestAsync(string versionId, string destinationPath)
    {
        using var httpClient = new HttpClient();
        string versionManifestUrl = "https://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
        string manifestJson = await httpClient.GetStringAsync(versionManifestUrl);

        // 解析找到指定版本
        using var doc = JsonDocument.Parse(manifestJson);
        var versions = doc.RootElement.GetProperty("versions");
        string? versionUrl = null;

        foreach (var version in versions.EnumerateArray())
        {
            if (version.GetProperty("id").GetString() == versionId)
            {
                versionUrl = version.GetProperty("url").GetString();
                break;
            }
        }

        if (versionUrl == null)
        {
            throw new InvalidOperationException($"找不到 Minecraft {versionId} 版本");
        }

        string versionJson = await httpClient.GetStringAsync(versionUrl);
        await File.WriteAllTextAsync(destinationPath, versionJson);
    }

    /// <summary>
    /// 下载客户端 JAR 文件
    /// </summary>
    private async Task DownloadClientJarAsync(string versionJsonPath, string jarPath)
    {
        // 读取版本配置获取客户端下载信息
        string versionContent = await File.ReadAllTextAsync(versionJsonPath);
        using var versionDoc = JsonDocument.Parse(versionContent);
        var downloads = versionDoc.RootElement.GetProperty("downloads");
        var client = downloads.GetProperty("client");
        string clientUrl = client.GetProperty("url").GetString() ?? throw new InvalidOperationException("Client URL is null");
        string clientSha1 = client.GetProperty("sha1").GetString() ?? throw new InvalidOperationException("Client SHA1 is null");

        try
        {
            // 下载客户端 JAR
            await _httpDownloader.DownloadFileAsync(clientUrl, jarPath, clientSha1);

            Console.WriteLine("客户端文件校验通过");
        }
        catch (Exception ex)
        {
            // 清理可能创建的部分文件
            try
            {
                if (File.Exists(jarPath))
                {
                    File.Delete(jarPath);
                }
            }
            catch
            {
                // 忽略清理错误
            }
            throw new Exception($"下载客户端失败: {ex.Message}", ex);
        }
    }
}