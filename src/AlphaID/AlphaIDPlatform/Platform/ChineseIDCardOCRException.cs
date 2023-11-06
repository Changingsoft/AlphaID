using System.Runtime.Serialization;

namespace AlphaIDPlatform.Platform;

/// <summary>
/// ID card recognize exception.
/// </summary>
[Serializable]
public class ChineseIdCardOcrException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public ChineseIdCardOcrException() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public ChineseIdCardOcrException(string message) : base(message) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public ChineseIdCardOcrException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ChineseIdCardOcrException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}