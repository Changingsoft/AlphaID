using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace IDSubjectsTests;
public class ADFSIdTokenSubGenerateTest
{
    /// <summary>
    /// 用于测试校准Microsoft AD FS生成id token的sub字段。
    /// <see cref="https://github.com/MicrosoftDocs/azure-docs/issues/16150"/>
    /// </summary>
    //[Fact]
    public void ADFSGenerateIdTokenSub()
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
