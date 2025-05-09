namespace AlphaIdPlatform.Platform;

/// <summary>
/// 一个无操作的短信验证码服务，用于调试或集成测试期间。该服务不会实际发送短信验证码，并且验证总是返回true。
/// </summary>
/// <remarks>
/// Ctor.
/// </remarks>
/// <param name="logger"></param>
public class NopVerificationCodeService(ILogger<NopVerificationCodeService> logger) : IVerificationCodeService
{
    /// <summary>
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    public Task SendAsync(string mobile)
    {
        logger.LogInformation("已模拟向{mobile}发送短信验证码。若使用该模拟服务验证验证码，请输入任意数字即可。", mobile);
        return Task.CompletedTask;
    }

    /// <summary>
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<bool> VerifyAsync(string mobile, string code)
    {
        logger.LogInformation("已验证{mobile}的验证码{code}。该模拟服务总是向调用方返回true。", mobile, code);
        return Task.FromResult(true);
    }
}