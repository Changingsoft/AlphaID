using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace IDSubjectsTests;
public class ADFSIdTokenSubGenerateTest
{
    /// <summary>
    /// <see cref="https://github.com/MicrosoftDocs/azure-docs/issues/16150"/>
    /// </summary>
    [Fact(Skip = "Not required.")]
    public void GenerateIdTokenSub()
    {
        var clientId = @"<client-id>";
        var anchorValue = @"<anchor-value>";
        var ppidPrivacyEntropy = @"<ppid-privacy-entropy>";

        var expected = "<expected>";

        byte[] originBytes = Array.Empty<byte>();
        originBytes = originBytes
            .Concat(Encoding.Unicode.GetBytes(clientId))
            .Concat(Encoding.Unicode.GetBytes(anchorValue))
            .Concat(Convert.FromBase64String(ppidPrivacyEntropy))
            .ToArray();
        var result = Convert.ToBase64String(SHA256.HashData(originBytes));

        Assert.Equal(expected, result);
    }
}
