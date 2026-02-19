using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Version;

public class VersionDownloader
{
    private readonly HttpDownloader _httpDownloader;

    public VersionDownloader(HttpDownloader httpDownloader)
    {
        _httpDownloader = httpDownloader;
    }

    public async Task DownloadVersionAsync(string minecraftRoot, string versionId, string versionUrl)
    {
        string versionsDir = Path.Combine(minecraftRoot, "versions", versionId);
        Directory.CreateDirectory(versionsDir);

        string jsonPath = Path.Combine(versionsDir, $"{versionId}.json");
        await _httpDownloader.DownloadFileAsync(versionUrl, jsonPath);

        // 下载对应的 jar 文件（可从 version.json 解析，简化处理）
        // 实际中可能需要解析 version.json 获取 client 下载地址
    }
}