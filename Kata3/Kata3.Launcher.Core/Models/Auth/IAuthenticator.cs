namespace Kata3.Launcher.Core.Models.Auth;

public interface IAuthenticator
{
    Task<Account> AuthenticateAsync();
}