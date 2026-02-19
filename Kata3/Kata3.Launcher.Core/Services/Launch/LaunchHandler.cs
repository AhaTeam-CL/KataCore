using Kata3.Launcher.Core.Models.Launch;
using Kata3.Launcher.Core.Services.Asset;
using Kata3.Launcher.Core.Services.Library;
using Kata3.Launcher.Core.Services.Version;

namespace Kata3.Launcher.Core.Services.Launch;

public class LaunchHandler
{
    private readonly VersionParser _versionParser;
    private readonly LibraryResolver _libraryResolver;
    private readonly LibraryDownloader _libraryDownloader;
    private readonly NativeExtractor _nativeExtractor;
    private readonly AssetManager _assetManager;
    private readonly ClassPathBuilder _classPathBuilder;
    private readonly ArgumentBuilder _argumentBuilder;
    private readonly ProcessLauncher _processLauncher;

    public LaunchHandler(
        VersionParser versionParser,
        LibraryResolver libraryResolver,
        LibraryDownloader libraryDownloader,
        NativeExtractor nativeExtractor,
        AssetManager assetManager,
        ClassPathBuilder classPathBuilder,
        ArgumentBuilder argumentBuilder,
        ProcessLauncher processLauncher)
    {
        _versionParser = versionParser;
        _libraryResolver = libraryResolver;
        _libraryDownloader = libraryDownloader;
        _nativeExtractor = nativeExtractor;
        _assetManager = assetManager;
        _classPathBuilder = classPathBuilder;
        _argumentBuilder = argumentBuilder;
        _processLauncher = processLauncher;
    }

    public async Task<LaunchResult> LaunchAsync(LaunchOptions options, IProgress<AssetDownloadProgress>? assetProgress = null)
    {
        try
        {
            string versionJsonPath = Path.Combine(options.MinecraftRoot, "versions", options.VersionId, $"{options.VersionId}.json");
            var version = await _versionParser.ParseAsync(versionJsonPath);

            var missingLibs = _libraryResolver.GetMissingLibraries(options.MinecraftRoot, version);
            if (missingLibs.Any())
            {
                await _libraryDownloader.DownloadLibrariesAsync(missingLibs, options.MinecraftRoot);
            }

            await _assetManager.EnsureAssetsAsync(options.MinecraftRoot, version, assetProgress);

            string nativesDir = await _nativeExtractor.ExtractAsync(options.MinecraftRoot, options.VersionId, version, options.UseVersionIsolation);

            string classPath = _classPathBuilder.Build(options.MinecraftRoot, version);

            var (jvmArgs, gameArgs) = _argumentBuilder.Build(options, version, classPath, nativesDir);

            var process = _processLauncher.Start(options.JavaPath, jvmArgs, gameArgs, options);

            return new LaunchResult { Process = process, Success = true };
        }
        catch (Exception ex)
        {
            return new LaunchResult { Success = false, Error = ex.Message };
        }
    }
}