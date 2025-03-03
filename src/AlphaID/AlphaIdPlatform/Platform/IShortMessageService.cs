namespace AlphaIdPlatform.Platform;

/// <summary>
/// Provide short message service.
/// </summary>
public interface IShortMessageService
{
    /// <summary>
    /// Send message.
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    Task SendAsync(string mobile, string content);
}