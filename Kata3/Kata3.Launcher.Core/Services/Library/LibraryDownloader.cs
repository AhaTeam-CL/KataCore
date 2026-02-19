using Kata3.Launcher.Core.Models.Minecraft;
using Kata3.Launcher.Core.Utilities;

namespace Kata3.Launcher.Core.Services.Library;

public class LibraryDownloader
{
    private readonly HttpDownloader _httpDownloader;

    public LibraryDownloader(HttpDownloader httpDownloader)
    {
        _httpDownloader = httpDownloader;
    }

    public async Task DownloadLibrariesAsync(List<Artifact> artifacts, string minecraftRoot)
    {
        var tasks = artifacts.Select(artifact =>
        {
            string path = Path.Combine(minecraftRoot, "libraries", artifact.Path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return _httpDownloader.DownloadFileAsync(artifact.Url, path, artifact.Sha1);
        });

        await Task.WhenAll(tasks);
    }
}