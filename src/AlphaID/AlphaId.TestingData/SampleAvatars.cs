using System.Reflection;

namespace AlphaId.TestingData;

/// <summary>
/// 读取随程序集嵌入的样例头像。
/// </summary>
/// <remarks>
/// 头像以二进制资源的形式放在 <c>Avatars</c> 目录下，随程序集发布。
/// 这样可以避免把二进制数据写成源码字面量，也不需要运行时存在某个外部目录。
/// </remarks>
internal static class SampleAvatars
{
    private const string Prefix = "AlphaId.TestingData.Avatars.";

    private static readonly Assembly Assembly = typeof(SampleAvatars).Assembly;

    /// <summary>
    /// 读取指定文件名的头像数据。
    /// </summary>
    /// <param name="fileName">文件名，例如 <c>liubei.jpg</c>。</param>
    /// <returns>头像的二进制内容。</returns>
    /// <exception cref="InvalidOperationException">未找到该资源。</exception>
    public static byte[] Read(string fileName)
    {
        using Stream? stream = Assembly.GetManifestResourceStream(Prefix + fileName);
        if (stream is null)
        {
            string available = string.Join(", ", Assembly.GetManifestResourceNames());
            throw new InvalidOperationException($"未找到样例头像资源“{fileName}”。现有资源：{available}");
        }

        using MemoryStream buffer = new();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }
}
