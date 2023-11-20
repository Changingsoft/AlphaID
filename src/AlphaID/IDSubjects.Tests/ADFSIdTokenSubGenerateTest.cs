using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace IdSubjects.Tests;
public class AdfsIdTokenSubGenerateTest
{
    /// <summary>
    /// 用于测试校准Microsoft AD FS生成id token的sub字段。
    /// <see href="https://github.com/MicrosoftDocs/azure-docs/issues/16150#issuecomment-429028832"/>
    /// </summary>
    [Fact]
    public void AdfsGenerateIdTokenSub()
    {
        var clientId = @"c1707efb-25d5-4999-954a-ede5a6a8cf51";
        var anchorValue = @"SpecifiedUserName";

        //这个值位于 AD FS 后端数据库，为固定值。SELECT TOP 1 [ServiceSettingsData] FROM [AdfsConfigurationV4].[IdentityServerPolicy].[ServiceSettings], XML Path: ServiceSettingsData/SecurityTokenService/PpidPrivacyEntropy
        //详见：https://github.com/MicrosoftDocs/azure-docs/issues/16150#issuecomment-429028832
        var ppidPrivacyEntropy = @"LKAi9pXlxmc7hnviBywEoHnZslIK9yjrufFQBoYd9BtLoO02o4yDwR7l/agyqvMDAADu8SAlwvnnrw9BVLaqY99h39VcsZjAaDyrEJBCP2ZHRA0S5kK8FTmKjs+qwNos3UPP44fvjzCrZ6q5GWfcN4gT4/yJmjgRrUmW5vpSZYVfIaPuutLOC1RPSveF8DJZL1pYHo4Ud6lkNLPP4FjqnzvlOPzsPM0WAE85r+Wsr3KA2xx3s6qhzD2+OP/aF1xXvOERn2qRd1NOpeIWU9sElJ0wKz9Lw0+9GKbYk3qhaotFo0s3EDa9CdYwVZ+DeSebisVUDbsqscjI0ccHxocz+A==";

        var expected = "e2eap+PlN90Mi5sSodneUrCZKBxlkdzSsoPpiHeC5VQ=";

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
