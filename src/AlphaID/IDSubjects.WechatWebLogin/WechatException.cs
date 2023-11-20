using System.Runtime.Serialization;

namespace IdSubjects.WechatWebLogin;

/// <summary>
/// 微信异常。
/// </summary>
[Serializable]
public class WechatException : Exception
{
    /// <summary>
    /// Initialize WechatException.
    /// </summary>
    public WechatException()
    {
    }

    /// <summary>
    /// Initialize Exception with Error Message.
    /// </summary>
    /// <param name="message"></param>
    public WechatException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initialize Exception with message and inner exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public WechatException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initialize exception for serialize.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected WechatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}