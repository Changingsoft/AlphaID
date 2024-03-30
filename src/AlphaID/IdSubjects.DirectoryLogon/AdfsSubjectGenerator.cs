using IdentityModel;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IdSubjects.DirectoryLogon;
internal class AdfsSubjectGenerator : ISubjectGenerator
{
    public string AnchorType { get; } = JwtClaimTypes.Subject;

    public string ClientIdType { get; } = JwtClaimTypes.ClientId;

    public string Generate(ClaimsPrincipal principal)
    {
        var anchorValue = principal.FindFirstValue(AnchorType) ?? throw new ArgumentException("Anchor value not found.");
        var clientId = principal.FindFirstValue(ClientIdType) ?? throw new ArgumentException("ClientId not found.");
        byte[] originBytes = [];
        originBytes =
        [
            .. originBytes,
            .. Encoding.Unicode.GetBytes(clientId),
            .. Encoding.Unicode.GetBytes(anchorValue),
            .. Convert.FromBase64String(PpidPrivacyEntropy),
        ];
        return Convert.ToBase64String(SHA256.HashData(originBytes));
    }

    private const string PpidPrivacyEntropy = @"LKAi9pXlxmc7hnviBywEoHnZslIK9yjrufFQBoYd9BtLoO02o4yDwR7l/agyqvMDAADu8SAlwvnnrw9BVLaqY99h39VcsZjAaDyrEJBCP2ZHRA0S5kK8FTmKjs+qwNos3UPP44fvjzCrZ6q5GWfcN4gT4/yJmjgRrUmW5vpSZYVfIaPuutLOC1RPSveF8DJZL1pYHo4Ud6lkNLPP4FjqnzvlOPzsPM0WAE85r+Wsr3KA2xx3s6qhzD2+OP/aF1xXvOERn2qRd1NOpeIWU9sElJ0wKz9Lw0+9GKbYk3qhaotFo0s3EDa9CdYwVZ+DeSebisVUDbsqscjI0ccHxocz+A==";
}
