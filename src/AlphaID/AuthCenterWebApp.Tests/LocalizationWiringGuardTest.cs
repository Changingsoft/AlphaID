using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AuthCenterWebApp.Tests;

/// <summary>
/// 守卫测试：验证 Program.cs 里真实装配出来的本地化链路（ResourcesPath、
/// DataAnnotationLocalizerProvider、IHtmlLocalizer 的注入点）确实能查到共享资源里的键。
/// 这是页面与数据注解真正会走的路径，比只测工厂更接近现场。
/// </summary>
[Collection(nameof(TestServerCollection))]
public class LocalizationWiringGuardTest(AuthCenterWebAppFactory factory)
{
    private const string ExpectedBaseName = "AuthCenterWebApp.Resources.SharedResource";

    [Fact]
    public void DiResolvesSharedResourceByType()
    {
        IStringLocalizer<SharedResource> localizer =
            factory.Services.GetRequiredService<IStringLocalizer<SharedResource>>();
        LocalizedString localized = localizer["Account"];
        Assert.False(localized.ResourceNotFound, $"SearchedLocation={localized.SearchedLocation}");
        Assert.Equal(ExpectedBaseName, localized.SearchedLocation);
    }

    [Fact]
    public void DiResolvesHtmlLocalizerUsedByViewImports()
    {
        // 各 _ViewImports.cshtml 里的 @inject IHtmlLocalizer<SharedResource> SL 走的就是这条路径。
        IHtmlLocalizer<SharedResource> localizer =
            factory.Services.GetRequiredService<IHtmlLocalizer<SharedResource>>();
        Assert.False(localizer["Account"].IsResourceNotFound);
    }

    [Fact]
    public void DataAnnotationLocalizerProviderResolvesSharedKeys()
    {
        MvcDataAnnotationsLocalizationOptions options = factory.Services
            .GetRequiredService<IOptions<MvcDataAnnotationsLocalizationOptions>>().Value;
        Assert.NotNull(options.DataAnnotationLocalizerProvider);

        IStringLocalizer localizer = options.DataAnnotationLocalizerProvider(
            typeof(string), factory.Services.GetRequiredService<IStringLocalizerFactory>());

        string[] keys = new[] { "Validate_Required", "Validate_StringLength" };
        foreach (string key in keys)
            Assert.False(localizer[key].ResourceNotFound, $"缺少数据注解共享键：{key}");
    }

    [Fact]
    public void PhoneNumberInvalidKeyStillResolvesAfterSignUpMigration()
    {
        // SignUp 页原先用自己的 IStringLocalizer<SignUpModel>，现已统一到共享资源里的 PhoneNumberInvalid 键。
        IStringLocalizer<SharedResource> localizer =
            factory.Services.GetRequiredService<IStringLocalizer<SharedResource>>();
        Assert.False(localizer["PhoneNumberInvalid"].ResourceNotFound);
    }
}
