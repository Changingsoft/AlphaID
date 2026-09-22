using Lazy.Captcha.Core;

namespace AuthCenterWebApp.CaptchaValidation;

/// <summary>
/// 图形验证码（Lazy.Captcha.Core）校验入口。
/// </summary>
/// <remarks>
/// <para>
/// 这里刻意不使用声明式的 <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute"/>：
/// Razor Pages 会在<b>每一个</b> POST 请求上绑定 <c>[BindProperty]</c> 属性并执行验证，
/// 其中也包括 <c>?handler=SendVerificationCode</c> 这类 AJAX 处理器。
/// 而 Lazy.Captcha.Core 的校验默认「用后即焚」，声明式校验会让「发送短信验证码」这一步
/// 提前消费掉验证码，导致随后的表单提交必然报「验证码无效」。
/// 因此在各页面的 OnPost 处理器内显式校验，以区分两种消费语义。
/// </para>
/// <para>
/// <b>不变量</b>：每个页面的表单 POST 处理器都必须在任何提前返回（<c>return Page()</c>）之前
/// 调用一次 <see cref="Validate"/>。这样才能保证「页面重新渲染」与「验证码已被消费」等价，
/// 视图里的 <c>_CaptchaScripts</c> 分部据此刷新验证码图片。
/// </para>
/// </remarks>
public static class CaptchaValidator
{
    /// <summary>
    /// 校验验证码，用于表单提交。无论成功或失败都会消费该验证码。
    /// </summary>
    /// <param name="captcha">验证码服务。</param>
    /// <param name="captchaId">验证码实例 ID，须与图片端点 <c>/captcha?id=</c> 使用的值一致。</param>
    /// <param name="code">用户输入的验证码。</param>
    public static bool Validate(ICaptcha captcha, string captchaId, string? code)
        => captcha.Validate(captchaId, code ?? string.Empty, removeIfSuccess: true, removeIfFail: true);

    /// <summary>
    /// 校验验证码，用于「发送短信验证码」这类两段式流程。
    /// 校验成功时<b>保留</b>验证码，使同一次页面交互中随后的表单提交仍能校验通过；
    /// 校验失败时仍然消费，避免该端点成为可反复猜测的暴力破解入口。
    /// </summary>
    /// <param name="captcha">验证码服务。</param>
    /// <param name="captchaId">验证码实例 ID，须与图片端点 <c>/captcha?id=</c> 使用的值一致。</param>
    /// <param name="code">用户输入的验证码。</param>
    public static bool ValidateKeepingOnSuccess(ICaptcha captcha, string captchaId, string? code)
        => captcha.Validate(captchaId, code ?? string.Empty, removeIfSuccess: false, removeIfFail: true);
}
