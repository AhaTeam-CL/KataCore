namespace Kata3.Launcher.Core.Models.Auth;

public class Account
{
    public required string UserName { get; set; }
    public Guid Uuid { get; set; } = Guid.Parse("00000000-ffff-ffff-ffff-ffffffff4de2");
    public string AccessToken { get; set; } = "00000FFFFFFFFFFFFFFFFFFFFFF4DE2A";
    public string ClientId { get; set; } = "0";
    public string Xuid { get; set; } = "0";
    public string UserType { get; set; } = "msa";
}