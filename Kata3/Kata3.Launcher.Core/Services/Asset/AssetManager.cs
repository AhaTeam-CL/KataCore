using Kata3.Launcher.Core.Models.Minecraft;

namespace Kata3.Launcher.Core.Services.Asset;

public class AssetManager
{
    private readonly AssetIndexLoader _indexLoader;
    private readonly AssetDownloader _downloader;

    public AssetManager(AssetIndexLoader indexLoader, AssetDownloader downloader)
    {
        _indexLoader = indexLoader;
        _downloader = downloader;
    }

    public async Task EnsureAssetsAsync(string minecraftRoot, VersionMeta version, IProgress<AssetDownloadProgress>? progress = null)
    {
        var assetIndex = await _indexLoader.LoadOrDownloadAsync(minecraftRoot, version);
        await _downloader.DownloadMissingAssetsAsync(minecraftRoot, assetIndex, progress);
    }
}