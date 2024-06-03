using System.Security.Cryptography;

namespace AdminWebApp.Areas.OpenIDConnect.Pages.Clients;

public class DefaultSecretGenerator : ISecretGenerator
{
    public string Generate()
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[24];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}