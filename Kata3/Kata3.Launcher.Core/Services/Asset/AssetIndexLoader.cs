using System.Text.Json;
using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Asset;

public class AssetIndexLoader
{
    private readonly HttpDownloader _httpDownloader;

    public AssetIndexLoader(HttpDownloader httpDownloader)
    {
        _httpDownloader = httpDownloader;
    }

    public async Task<AssetIndex> LoadOrDownloadAsync(string minecraftRoot, VersionMeta version)
    {
        var assetIndexRef = version.AssetIndex;
        if (assetIndexRef == null)
            throw new InvalidOperationException("Version does not have an asset index.");

        string indexPath = Path.Combine(minecraftRoot, "assets", "indexes", $"{assetIndexRef.Id}.json");

        if (!File.Exists(indexPath) || new FileInfo(indexPath).Length != assetIndexRef.Size)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(indexPath));
            await _httpDownloader.DownloadFileAsync(assetIndexRef.Url, indexPath, assetIndexRef.Sha1);
        }

        await using var stream = File.OpenRead(indexPath);
        return await JsonSerializer.DeserializeAsync<AssetIndex>(stream);
    }
}