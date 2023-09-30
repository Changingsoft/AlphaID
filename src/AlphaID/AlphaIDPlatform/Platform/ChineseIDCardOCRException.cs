using System.Runtime.Serialization;

namespace AlphaIDPlatform.Platform;

/// <summary>
/// ID card recognize exception.
/// </summary>
[Serializable]
public class ChineseIDCardOCRException : Exception
{
    /// <summary>
    /// 
    /// </summary>
    public ChineseIDCardOCRException() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public ChineseIDCardOCRException(string message) : base(message) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public ChineseIDCardOCRException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected ChineseIDCardOCRException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}