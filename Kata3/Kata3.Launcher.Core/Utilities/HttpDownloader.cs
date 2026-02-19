using System.Net.Http;
using System.Security.Cryptography;

namespace Kata3.Launcher.Core.Utilities;

public class HttpDownloader : IDisposable
{
    private readonly HttpClient _httpClient;

    public HttpDownloader()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Kata3-Launcher/1.0");
    }

    public async Task DownloadFileAsync(string url, string destinationPath, string? expectedSha1 = null, CancellationToken token = default)
    {
        string tempPath = destinationPath + ".tmp";
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!); // 消除警告

        // 确保临时文件不存在
        if (File.Exists(tempPath))
        {
            try
            {
                File.Delete(tempPath);
            }
            catch
            {
                // 如果删除失败，尝试等待并重试
                await Task.Delay(100, token);
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        try
        {
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();

                await using var contentStream = await response.Content.ReadAsStreamAsync(token);
                await using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
                await contentStream.CopyToAsync(fileStream, token);
            }

            if (!string.IsNullOrEmpty(expectedSha1))
            {
                string actualSha1 = await ComputeSha1Async(tempPath);
                if (!actualSha1.Equals(expectedSha1, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(tempPath);
                    throw new InvalidDataException($"SHA1 mismatch for {url}. Expected {expectedSha1}, got {actualSha1}.");
                }
            }

            // 安全地移动文件
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            File.Move(tempPath, destinationPath);
        }
        catch
        {
            // 清理临时文件
            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch
            {
                // 忽略清理错误
            }
            throw;
        }
    }

    private static async Task<string> ComputeSha1Async(string filePath)
    {
        using var sha1 = SHA1.Create();
        await using var stream = File.OpenRead(filePath);
        byte[] hash = await sha1.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}