using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AuthCenterWebApp.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCenterWebApp.Tests.Endpoints;

/// <summary>
/// <c>/connect/token</c> 的限速测试。
/// </summary>
/// <remarks>
/// 它<b>刻意不挂在 <see cref="TestServerCollection"/> 上</b>，而是用自己的
/// <see cref="RateLimitedTokenEndpointFactory"/>，理由见该工厂的说明。
/// </remarks>
public class TokenEndpointRateLimitTest(RateLimitedTokenEndpointFactory factory)
    : IClassFixture<RateLimitedTokenEndpointFactory>
{
    /// <summary>一个能成功换到令牌的 client_credentials 请求。</summary>
    private static readonly Dictionary<string, string> ValidClientCredentials = new()
    {
        { "client_id", "d70700eb-c4d8-4742-a79a-6ecf2064b27c" },
        { "client_secret", "i7zcwJu)5pgIA()huJWRoT@oCLHpwfe^" },
        { "grant_type", "client_credentials" },
    };

    /// <summary>
    /// 前 <c>PermitLimit</c> 次请求必须成功，第 <c>PermitLimit + 1</c> 次必须被限速拒绝。
    /// </summary>
    [Fact]
    public async Task Token_IsRejectedOncePermitLimitIsExceeded()
    {
        HttpClient client = factory.Client;

        // 配额内的请求必须真的换到令牌——否则「拿到 429」可能只是因为客户端凭据失效，
        // 那种情况下限速测试会假通过。
        for (int i = 0; i < RateLimitedTokenEndpointFactory.PermitLimit; i++)
        {
            using HttpResponseMessage allowed = await client.PostAsync(
                "/connect/token",
                new FormUrlEncodedContent(ValidClientCredentials),
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
            var token = await allowed.Content.ReadFromJsonAsync<TokenResponse>(TestContext.Current.CancellationToken);
            Assert.False(string.IsNullOrEmpty(token?.AccessToken));
        }

        using HttpResponseMessage rejected = await client.PostAsync(
            "/connect/token",
            new FormUrlEncodedContent(ValidClientCredentials),
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
    }

    public record TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = null!;

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = null!;
    }
}

/// <summary>
/// 一个把 token 端点限速配额改成极小值的独立宿主。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要独立宿主。</b>限速按 <c>RemoteIpAddress</c> 分区，TestServer 下所有请求的远端地址
/// 都是 localhost，也就是同一个 <see cref="AuthCenterWebAppFactory"/> 里的所有测试
/// <b>共用同一个配额桶</b>。限速测试只要把桶打空，随后所有打 <c>/connect/token</c> 的测试
/// 都会收到 429，且失败原因看起来像业务 bug。
/// </para>
/// <para>
/// 这里必须能覆盖配额，有赖于生产侧把 <c>static</c> 限速器改成了实例字段 + 可配置的
/// <see cref="TokenRateLimitOptions"/>。<b>static 的限速器跨所有宿主实例共享</b>，
/// 那样测试就只能去撞生产的 10 次/分钟：既慢，又打空整个进程的桶。
/// </para>
/// <para>
/// 本类<b>不改动任何产品代码</b>：配额本来就是从配置节绑定的，这里只是覆盖它的值。
/// </para>
/// </remarks>
public class RateLimitedTokenEndpointFactory : AuthCenterWebAppFactory
{
    /// <summary>测试专用的窗口内请求配额；取得足够小，让测试只需发几次就能触顶。</summary>
    public const int PermitLimit = 3;

    /// <summary>与生产配置一致的窗口长度；此处不验证窗口过期（那需要真的等 1 分钟）。</summary>
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    private HttpClient? _client;

    /// <summary>复用一个客户端，保持连接池稳定。</summary>
    public HttpClient Client => _client ??= CreateClient();

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.Configure<TokenRateLimitOptions>(options =>
            {
                options.PermitLimit = PermitLimit;
                options.Window = Window;
            });
        });
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _client?.Dispose();
            _client = null;
        }

        base.Dispose(disposing);
    }
}