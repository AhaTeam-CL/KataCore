using System.Security.Cryptography;
using System.Text;
using Kata3.Launcher.Core.Models.Auth;

namespace Kata3.Launcher.Core.Services.Auth;

public class OfflineAuthenticator : IAuthenticator
{
    private readonly string _username;

    public OfflineAuthenticator(string username)
    {
        _username = username;
    }

    public Task<Account> AuthenticateAsync()
    {
        // 使用 MD5 生成与 Minecraft 官方离线模式兼容的 UUID
        Guid uuid = GenerateOfflineUuid(_username);

        var account = new Account
        {
            UserName = _username,
            Uuid = uuid,
            AccessToken = "0",
            UserType = "legacy"
        };
        return Task.FromResult(account);
    }

    private static Guid GenerateOfflineUuid(string username)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes("OfflinePlayer:" + username));
        // 设置版本为 3 (MD5) 和变体为 RFC 4122
        hash[6] = (byte)((hash[6] & 0x0F) | 0x30);
        hash[8] = (byte)((hash[8] & 0x3F) | 0x80);
        return new Guid(hash);
    }
}