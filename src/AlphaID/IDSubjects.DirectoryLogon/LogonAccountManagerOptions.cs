using System.DirectoryServices;
using System.Security.Cryptography;
using System.Text;

namespace IDSubjects.DirectoryLogon;

/// <summary>
/// LogonAccount Manager Options
/// </summary>
public class LogonAccountManagerOptions
{
    /// <summary>
    /// 在ADFS中注册的ClientId.
    /// </summary>
    public string ClientId { get; set; } = @"8cdd4862-37bd-462f-a1cc-698c59a0c7fe";

    /// <summary>
    /// 锚定属性名称。
    /// </summary>
    public string AnchorPropertyName { get; set; } = "sAMAccountName";

    /// <summary>
    /// 隐私熵。
    /// </summary>
    public string PpidPrivacyEntropy { get; set; } = @"LKAi9pXlxmc7hnviBywEoHnZslIK9yjrufFQBoYd9BtLoO02o4yDwR7l/agyqvMDAADu8SAlwvnnrw9BVLaqY99h39VcsZjAaDyrEJBCP2ZHRA0S5kK8FTmKjs+qwNos3UPP44fvjzCrZ6q5GWfcN4gT4/yJmjgRrUmW5vpSZYVfIaPuutLOC1RPSveF8DJZL1pYHo4Ud6lkNLPP4FjqnzvlOPzsPM0WAE85r+Wsr3KA2xx3s6qhzD2+OP/aF1xXvOERn2qRd1NOpeIWU9sElJ0wKz9Lw0+9GKbYk3qhaotFo0s3EDa9CdYwVZ+DeSebisVUDbsqscjI0ccHxocz+A==";

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    internal string GenerateExternalLoginId(string sAmDomainPart, DirectoryEntry userEntry)
    {
        var anchorValue = $"{sAmDomainPart}\\{userEntry.Properties[this.AnchorPropertyName].Value}";
        byte[] originBytes = Array.Empty<byte>();
        originBytes = originBytes
            .Concat(Encoding.Unicode.GetBytes(this.ClientId))
            .Concat(Encoding.Unicode.GetBytes(anchorValue))
            .Concat(Convert.FromBase64String(this.PpidPrivacyEntropy))
            .ToArray();
        return Convert.ToBase64String(SHA256.HashData(originBytes));
    }
}