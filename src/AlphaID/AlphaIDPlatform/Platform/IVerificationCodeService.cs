namespace AlphaIDPlatform.Platform;

/// <summary>
/// Provide verification code service.
/// </summary>
public interface IVerificationCodeService
{
    /// <summary>
    /// Send verification code.
    /// </summary>
    /// <param name="mobile"></param>
    /// <returns></returns>
    Task SendAsync(string mobile);

    /// <summary>
    /// Verify code with mobile.
    /// </summary>
    /// <param name="mobile"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<bool> VerifyAsync(string mobile, string code);
}