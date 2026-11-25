using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;

namespace AuthCenterWebApp.Tests;

/// <summary>
/// 守卫测试：<see cref="SharedResource"/> 这个「锚点类」必须留在项目根命名空间，且工厂由它推导出的
/// 资源基名必须等于程序集里真实的清单资源名。
/// </summary>
/// <remarks>
/// <para>
/// 这类配置一旦错位，<c>ResourceManagerStringLocalizer</c> 会吞掉 <c>MissingManifestResourceException</c>，
/// 既不抛异常也不写日志，页面只会把键名原样显示出来（例如 <c>Account</c>、<c>Validate_Required</c>），
/// 构建与测试照样通过。所以必须用断言把它变成红灯。
/// </para>
/// <para>详见 <c>docs/Development.md</c> 的「共享资源与锚点类」一节。</para>
/// </remarks>
public class SharedResourceLocalizationGuardTest
{
    private const string ExpectedAssemblyName = "AuthCenterWebApp";
    private const string ExpectedBaseName = "AuthCenterWebApp.Resources.SharedResource";

    /// <summary>必须与 Program.cs 中 AddLocalization 的 ResourcesPath 一致。</summary>
    private const string ResourcesPath = "Resources";

    private static IStringLocalizerFactory CreateFactory()
        => new ResourceManagerStringLocalizerFactory(
            Options.Create(new LocalizationOptions { ResourcesPath = ResourcesPath }),
            NullLoggerFactory.Instance);

    [Fact]
    public void AnchorTypeSitsInRootNamespace()
    {
        // 若把 SharedResource.cs 移入 Resources/ 目录，IDE 会把它的命名空间同步成 AuthCenterWebApp.Resources，
        // 基名随之变成 AuthCenterWebApp.Resources.Resources.SharedResource，所有查找静默落空。
        Assert.Equal(ExpectedAssemblyName, typeof(SharedResource).Namespace);
    }

    [Fact]
    public void AssemblyNameIsTheLocalizationRootNamespace()
    {
        Assembly assembly = typeof(SharedResource).Assembly;
        Assert.Equal(ExpectedAssemblyName, assembly.GetName().Name);
        // SDK 不生成该特性，此时根命名空间回退为程序集名；一旦被人手工加上，两者就会脱钩。
        Assert.Null(assembly.GetCustomAttribute<RootNamespaceAttribute>());
    }

    [Fact]
    public void SharedResourceIsEmbeddedUnderExpectedName()
    {
        string[] names = typeof(SharedResource).Assembly.GetManifestResourceNames();
        Assert.Contains($"{ExpectedBaseName}.resources", names);
    }

    [Fact]
    public void ChineseSatelliteContainsSharedResource()
    {
        // 中文资源在卫星程序集里；这里能抓到「zh-CN 资源被误删/未嵌入」。
        // 注意：目录名的大小写在 Windows 上不敏感（实际输出目录是 zh-cn），故本测试抓不到大小写问题。
        Assembly satellite = typeof(SharedResource).Assembly.GetSatelliteAssembly(new CultureInfo("zh-CN"));
        string[] names = satellite.GetManifestResourceNames();
        Assert.Contains(names,
            n => n.Equals($"{ExpectedBaseName}.zh-CN.resources", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void FactoryResolvesKeysAndReportsBaseName()
    {
        LocalizedString localized = CreateFactory().Create(typeof(SharedResource))["Account"];
        Assert.False(localized.ResourceNotFound,
            $"键 '{localized.Name}' 未命中，SearchedLocation={localized.SearchedLocation}");
        Assert.Equal(ExpectedBaseName, localized.SearchedLocation);
    }

    [Fact]
    public void DataAnnotationSharedKeysResolve()
    {
        IStringLocalizer localizer = CreateFactory().Create(typeof(SharedResource));
        // 注意：DataAnnotationLocalizerProvider 是 (_, f) => f.Create(typeof(SharedResource))，忽略类型参数，
        // 因此本 WebApp 里所有 [Required]/[StringLength]/[Display] 的键都必须存在于 SharedResource.resx。
        // 已知缺口（既有问题，本次未修）：代码引用了 Validate_EmailFormat（4 处）与 Validate_MaxLength（1 处），
        // 但本项目的 SharedResource.resx 里没有这两个键，故这些校验消息目前会显示为键名本身。
        string[] keys = new[]
        {
            "Validate_Required",
            "Validate_StringLength",
            "Validate_EmailAddress",
            "Validate_PasswordConfirm",
            "Captcha_Invalid",
            "Invalid_Credentials",
            "PhoneNumberInvalid"
        };
        foreach (string key in keys)
            Assert.False(localizer[key].ResourceNotFound, $"缺少数据注解共享键：{key}");
    }

    [Fact]
    public void PageCodeBehindsUseSharedResourceOnly()
    {
        // 曾经的 Resources/SharedResource.Designer.cs（internal class SharedResource，命名空间
        // AuthCenterWebApp.Resources）与锚点同名不同物，是第二个「同一个清单、两处声明」的来源，现已删除；
        // 原 Resources/Pages/Account/SignUpModel.resx 也已并入 SharedResource。
        string[] names = typeof(SharedResource).Assembly.GetManifestResourceNames();
        Assert.DoesNotContain("AuthCenterWebApp.Resources.Pages.Account.SignUpModel.resources", names);
        Assert.DoesNotContain("AuthCenterWebApp.Resources.Pages.Account.SignUpModel.zh-CN.resources", names);
    }
}
