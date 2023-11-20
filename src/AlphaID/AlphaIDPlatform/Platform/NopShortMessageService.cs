namespace AlphaIdPlatform.Platform;

/// <summary>
/// 一个无操作的短信服务，用于调试或集成测试。该服务不会实际发送短信，它将生成一条消息性日志。
/// </summary>
public class NopShortMessageService : IShortMessageService
{
    private readonly ILogger<NopShortMessageService> logger;

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="logger"></param>
    public NopShortMessageService(ILogger<NopShortMessageService> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public Task SendAsync(string mobile, string content)
    {
        this.logger.LogInformation("已模拟向{mobile}发送自由文本短信，内容是：{content}", mobile, content);
        return Task.CompletedTask;
    }
}
