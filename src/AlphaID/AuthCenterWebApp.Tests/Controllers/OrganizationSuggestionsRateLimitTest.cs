using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.RateLimiting;

namespace AuthCenterWebApp.Tests.Controllers;

/// <summary>
/// 组织搜索端点的限速测试。
/// </summary>
/// <remarks>
/// <para>
/// 它<b>刻意不挂在 <see cref="TestServerCollection"/> 上</b>，而是用自己的
/// <see cref="RateLimitedAuthCenterWebAppFactory"/>。原因见该工厂的说明——
/// 限速配额是按 IP 分区的共享状态，共用 fixture 会让限速测试把配额耗光，
/// 从而让同一集合里其它测试全部拿到 429。这是本文件原先那个测试被跳过的真正原因：
/// 它不是「跑不过」，而是「跑起来会污染别人」。
/// </para>
/// <para>
/// 隔离之后，请求数从 300 降到 <see cref="RateLimitedAuthCenterWebAppFactory.PermitLimit"/> + 1
/// （默认 4 个），断言也从「第 300 个是 429」变成「前 N 个成功、第 N+1 个被拒」——
/// 后者才能真正锁住 <c>PermitLimit</c> 的取值。
/// </para>
/// </remarks>
public class OrganizationSuggestionsRateLimitTest(RateLimitedAuthCenterWebAppFactory factory)
    : IClassFixture<RateLimitedAuthCenterWebAppFactory>
{
    /// <summary>
    /// 前 <c>PermitLimit</c> 次请求必须成功，第 <c>PermitLimit + 1</c> 次必须被限速拒绝。
    /// </summary>
    [Fact]
    public async Task Suggestions_IsRejectedOncePermitLimitIsExceeded()
    {
        HttpClient client = factory.CreateClient();
        string url = $"/api/Organization/Suggestions?q={WebUtility.UrlEncode("蜀汉")}";

        // 配额内的请求必须真的成功——否则「拿到 429」可能只是因为端点坏了，
        // 那种情况下限速测试会假通过。
        for (int i = 0; i < RateLimitedAuthCenterWebAppFactory.PermitLimit; i++)
        {
            using HttpResponseMessage allowed = await client.GetAsync(url, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        }

        using HttpResponseMessage rejected = await client.GetAsync(url, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
    }
}

/// <summary>
/// 一个把 <c>ip-fixed</c> 限速策略改成极小配额的独立宿主。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要独立宿主。</b>限速器按 <c>RemoteIpAddress</c> 分区，而 TestServer 下所有请求的
/// 远端地址都是 localhost。也就是说，同一个 <see cref="AuthCenterWebAppFactory"/> 里的所有测试
/// ——包括 <c>OrganizationControllerTest</c> 的正常用例——<b>共用同一个 100 次/分钟的配额桶</b>。
/// 限速测试只要把桶打空，随后所有打这个端点的测试都会收到 429，且失败原因看起来像业务 bug，
/// 排查成本极高。
/// </para>
/// <para>
/// 独立的 <c>WebApplicationFactory</c> 有自己的 DI 容器，因此有<b>自己的限速器实例与配额桶</b>，
/// 与其它测试完全隔离。<see cref="TokenEndpointTest"/> 做不到这一点，原因见那里的说明。
/// </para>
/// <para>
/// <b>怎么改配额。</b><c>RateLimiterOptions.AddPolicy</c> 内部是 <c>TryAdd</c>，
/// 对已存在的策略名是<b>静默无效</b>的——直接 <c>PostConfigure</c> 里再 <c>AddPolicy</c> 不会生效。
/// 必须先 <see cref="RateLimiterOptions.RemovePolicy"/> 再 <c>AddPolicy</c>。
/// </para>
/// <para>
/// 本类<b>不改动任何产品代码</b>。若将来把限速参数改为可配置（<c>RateLimiting:*</c> 配置节），
/// 这里就可以退化成只覆盖配置值，更简单也更贴近生产行为。
/// </para>
/// </remarks>
public class RateLimitedAuthCenterWebAppFactory : AuthCenterWebAppFactory
{
    /// <summary>
    /// 测试专用的每分钟请求配额。取得足够小，让测试只需发几次请求就能触顶。
    /// </summary>
    public const int PermitLimit = 3;

    /// <summary>与生产配置保持一致的窗口长度；此处不验证窗口过期（那需要真的等 1 分钟）。</summary>
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.PostConfigure<RateLimiterOptions>(options =>
            {
                TryRemovePolicy(options, "ip-fixed");
                options.AddPolicy("ip-fixed", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = PermitLimit,
                        Window = Window,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0,
                    }));
            });
        });
    }

    /// <summary>
    /// 从 <paramref name="options"/> 里移除一个已注册的限速策略。
    /// </summary>
    /// <param name="options">待调整的选项。</param>
    /// <param name="policyName">策略名。</param>
    /// <remarks>
    /// <para>
    /// 为什么需要它：<c>RateLimiterOptions.AddPolicy</c> 对<b>已存在</b>的策略名会直接抛
    /// <c>There already exists a policy with the name …</c>。也就是说，直接在
    /// <c>PostConfigure</c> 里再 <c>AddPolicy("ip-fixed", …)</c> 不是「覆盖」而是「炸」；
    /// 而绕开它、沿用生产的 100 次配额，就意味着测试要么打不顶（假通过）、
    /// 要么得发 100+ 个请求（就是原来那个测试慢又污染别人的原因）。必须先移除。
    /// </para>
    /// <para>
    /// 为什么用反射：<c>RateLimiterOptions</c> 在 .NET 10 上有 <c>RemovePolicy</c>，
    /// <b>但 net8.0 上没有</b>，而本工程多目标（net8.0;net10.0）。而策略字典
    /// <c>PolicyMap</c> 是 private 的，只读不写。
    /// </para>
    /// <para>
    /// <b>目标是 <c>PolicyMap</c> 而不是 <c>UnactivatedPolicyMap</c></b>（实测确认）：
    /// 后者是给「尚未激活的策略」用的空字典，<c>AddPolicy</c> 实际写入的是前者。
    /// 取错了字典不会报错，只会让移除静默无效，随后 <c>AddPolicy</c> 抛「已存在同名策略」——
    /// 这正是本方法要在拿不到字典时<b>显式抛异常</b>的原因：宁可响亮失败，也不要限速测试假通过。
    /// </para>
    /// </remarks>
    private static void TryRemovePolicy(RateLimiterOptions options, string policyName)
    {
        PropertyInfo? mapProperty = typeof(RateLimiterOptions)
            .GetProperty("PolicyMap", BindingFlags.Instance | BindingFlags.NonPublic);

        if (mapProperty?.GetValue(options) is not System.Collections.IDictionary policies)
        {
            throw new InvalidOperationException(
                $"无法移除限速策略「{policyName}」：RateLimiterOptions.PolicyMap 不可访问。" +
                "该测试会因限速器仍使用生产配额而失去意义，请更新此处的反射以适配新的框架版本。");
        }

        policies.Remove(policyName);
    }
}
