using Kata3.Launcher.Core.Models.Launch;
using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Services.Asset;
using Kata3.Launcher.Core.Services.Launch;
using Kata3.Launcher.Core.Services.Library;
using Kata3.Launcher.Core.Services.Version;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core;

public class MinecraftLauncher
{
    private readonly LaunchHandler _launchHandler;
    private readonly VersionManager _versionManager;
    private readonly VersionParser _versionParser;

    public MinecraftLauncher()
    {
        var osHelper = new OsHelper();
        var httpDownloader = new HttpDownloader();
        
        _versionParser = new VersionParser();
        _versionManager = new VersionManager(httpDownloader, _versionParser);

        var libraryResolver = new LibraryResolver(osHelper);
        var libraryDownloader = new LibraryDownloader(httpDownloader);
        var nativeExtractor = new NativeExtractor(osHelper);
        
        var assetIndexLoader = new AssetIndexLoader(httpDownloader);
        var assetDownloader = new AssetDownloader(httpDownloader, maxConcurrency: 8);
        var assetManager = new AssetManager(assetIndexLoader, assetDownloader);

        var classPathBuilder = new ClassPathBuilder(osHelper);
        var argumentBuilder = new ArgumentBuilder(osHelper);
        var processLauncher = new ProcessLauncher();

        _launchHandler = new LaunchHandler(
            _versionParser,
            libraryResolver,
            libraryDownloader,
            nativeExtractor,
            assetManager,
            classPathBuilder,
            argumentBuilder,
            processLauncher);
    }

    /// <summary>
    /// 启动 Minecraft 游戏
    /// </summary>
    /// <param name="options">启动选项</param>
    /// <param name="assetProgress">资源下载进度回调</param>
    /// <returns>启动结果</returns>
    public async Task<LaunchResult> LaunchAsync(LaunchOptions options, IProgress<AssetDownloadProgress>? assetProgress = null)
    {
        if (!Directory.Exists(options.WorkingDirectory))
            Directory.CreateDirectory(options.WorkingDirectory);

        // 确保游戏版本完整性
        await _versionManager.EnsureVersionIntegrityAsync(options.MinecraftRoot, options.VersionId);

        return await _launchHandler.LaunchAsync(options, assetProgress);
    }

    /// <summary>
    /// 仅检查并补全游戏文件，不启动游戏
    /// </summary>
    /// <param name="minecraftRoot">Minecraft 根目录</param>
    /// <param name="versionId">版本 ID</param>
    /// <returns>版本元数据</returns>
    public async Task<VersionMeta> EnsureGameIntegrityAsync(string minecraftRoot, string versionId)
    {
        return await _versionManager.EnsureVersionIntegrityAsync(minecraftRoot, versionId);
    }
}