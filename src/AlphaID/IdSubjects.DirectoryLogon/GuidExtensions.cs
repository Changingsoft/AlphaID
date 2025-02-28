using System.Text;

namespace IdSubjects.DirectoryLogon;

internal static class GuidExtensions
{
    /// <summary>
    /// Gets values format like "\01\23\45\67\89\0A\BC..." from Guid.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static string ToHexString(this Guid guid)
    {
        var sb = new StringBuilder();
        byte[] bytes = guid.ToByteArray();
        foreach (byte b in bytes) sb.Append($"\\{b:X2}");
        return sb.ToString();
    }
}