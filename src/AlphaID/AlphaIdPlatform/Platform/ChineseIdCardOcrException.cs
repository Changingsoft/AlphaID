namespace AlphaIdPlatform.Platform;

/// <summary>
/// ID card recognize exception.
/// </summary>
[Serializable]
public class ChineseIdCardOcrException : Exception
{
    /// <summary>
    /// </summary>
    public ChineseIdCardOcrException()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    public ChineseIdCardOcrException(string message) : base(message)
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public ChineseIdCardOcrException(string message, Exception inner) : base(message, inner)
    {
    }
}