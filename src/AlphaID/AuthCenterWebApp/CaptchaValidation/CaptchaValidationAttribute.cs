using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Resources;
using Lazy.Captcha.Core;

namespace AuthCenterWebApp.CaptchaValidation;

/// <summary>
/// 基于 Lazy.Captcha.Core 的图形验证码验证特性，替代 BotDetect 的 <c>CaptchaModelStateValidation</c>。
/// <see cref="ValidationAttribute.ErrorMessage"/> 沿用 SharedResource 资源键（如 "Captcha_Invalid"）。
/// 自定义 ValidationAttribute 不经过 MVC 的 DataAnnotations 适配器，本地化需在此手动完成。
/// 注意：不能用 IStringLocalizer&lt;SharedResource&gt;——本项目 AddLocalization 配置了 ResourcesPath="Resources"，
/// 而 SharedResource 类型本身位于 Resources 命名空间下，工厂会把路径拼成
/// "AuthCenterWebApp.Resources.Resources.SharedResource" 导致永远查不到。
/// 此处按嵌入资源名直接构造 ResourceManager，与 SharedResource.Designer.cs 同源。
/// </summary>
/// <param name="captchaId">验证码实例 ID，须与图片端点使用的 id 一致（如 "PhoneLogin"）。</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CaptchaValidationAttribute(string captchaId) : ValidationAttribute
{
    private static ResourceManager? _resourceManager;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var captcha = validationContext.GetService(typeof(ICaptcha)) as ICaptcha
            ?? throw new InvalidOperationException("没有注册 Lazy.Captcha.Core 验证码服务（AddCaptcha）。");

        if (captcha.Validate(captchaId, value as string ?? string.Empty))
            return ValidationResult.Success;

        string key = ErrorMessage ?? "Captcha_Invalid";
        string message = GetLocalizedMessage(key);
        return new ValidationResult(message);
    }

    private static string GetLocalizedMessage(string key)
    {
        _resourceManager ??= new ResourceManager(
            typeof(Resources.SharedResource).Namespace + "." + nameof(Resources.SharedResource),
            typeof(Resources.SharedResource).Assembly);

        return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
