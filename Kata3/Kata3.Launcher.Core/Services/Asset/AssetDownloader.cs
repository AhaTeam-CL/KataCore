using System.Collections.Concurrent;
using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Asset;

public class AssetDownloadProgress
{
    public int Completed { get; set; }
    public int Total { get; set; }
    public double Percentage => (double)Completed / Total * 100;
}

public class AssetDownloader
{
    private readonly HttpDownloader _httpDownloader;
    private readonly int _maxConcurrency;

    public AssetDownloader(HttpDownloader httpDownloader, int maxConcurrency = 8)
    {
        _httpDownloader = httpDownloader;
        _maxConcurrency = maxConcurrency;
    }

    public async Task DownloadMissingAssetsAsync(string minecraftRoot, AssetIndex assetIndex, IProgress<AssetDownloadProgress>? progress = null)
    {
        string objectsDir = Path.Combine(minecraftRoot, "assets", "objects");

        var toDownload = new ConcurrentBag<KeyValuePair<string, AssetObject>>();
        foreach (var kv in assetIndex.Objects)
        {
            string hash = kv.Value.Hash;
            string prefix = hash[..2];
            string assetPath = Path.Combine(objectsDir, prefix, hash);
            if (!File.Exists(assetPath) || new FileInfo(assetPath).Length != kv.Value.Size)
            {
                toDownload.Add(kv);
            }
        }

        int total = toDownload.Count;
        if (total == 0) return;

        int completed = 0;
        var options = new ParallelOptions { MaxDegreeOfParallelism = _maxConcurrency };

        await Parallel.ForEachAsync(toDownload, options, async (kv, token) =>
        {
            string hash = kv.Value.Hash;
            string prefix = hash[..2];
            string url = $"https://resources.download.minecraft.net/{prefix}/{hash}";
            string destPath = Path.Combine(objectsDir, prefix, hash);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                await _httpDownloader.DownloadFileAsync(url, destPath, hash, token);

                int current = Interlocked.Increment(ref completed);
                progress?.Report(new AssetDownloadProgress { Completed = current, Total = total });
            }
            catch (Exception ex)
            {
                // 记录错误但继续处理其他文件
                Console.WriteLine($"警告: 下载资源 {kv.Key} 失败: {ex.Message}");
                
                // 仍然增加计数器以避免进度卡住
                int current = Interlocked.Increment(ref completed);
                progress?.Report(new AssetDownloadProgress { Completed = current, Total = total });
            }
        });
    }
}