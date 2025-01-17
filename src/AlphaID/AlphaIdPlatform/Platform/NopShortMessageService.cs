namespace AlphaIdPlatform.Platform;

/// <summary>
///     一个无操作的短信服务，用于调试或集成测试。该服务不会实际发送短信，它将生成一条消息性日志。
/// </summary>
/// <remarks>
///     Ctor.
/// </remarks>
/// <param name="logger"></param>
public class NopShortMessageService(ILogger<NopShortMessageService> logger) : IShortMessageService
{
    /// <summary>
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public Task SendAsync(string mobile, string content)
    {
        logger.LogInformation("已模拟向{mobile}发送自由文本短信，内容是：{content}", mobile, content);
        return Task.CompletedTask;
    }
}