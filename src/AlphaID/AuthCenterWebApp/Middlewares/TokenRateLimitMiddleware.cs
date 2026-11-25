using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace AuthCenterWebApp.Middlewares
{
    /// <summary>
    /// 针对 /connect/token 的基于IP的速率限制。
    /// </summary>
    public class TokenRateLimitOptions
    {
        /// <summary>配置节名。</summary>
        public const string SectionName = "TokenRateLimit";

        /// <summary>每个窗口内允许的请求数。</summary>
        public int PermitLimit { get; set; } = 10;

        /// <summary>窗口长度。</summary>
        public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// 针对 /connect/token 的基于IP的速率限制中间件。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 为什么不用框架的限速器（<c>AddRateLimiter</c> + 策略）而要自己写中间件：
    /// <c>/connect/token</c> 是由 Duende IdentityServer 映射的端点，它没有控制器、
    /// 也就没有可以挂 <c>[EnableRateLimiting]</c> 特性的地方。中间件是这里唯一可靠的挂载点。
    /// </para>
    /// <para>
    /// <b>配额刻意做成实例字段而不是 static。</b>static 限速器会在<b>整个进程</b>内共享：
    /// 集成测试里每个 <c>WebApplicationFactory</c> 都是独立的宿主，但 static 的桶是同一个，
    /// 于是测试无法把自己的配额调小——只能去撞生产的 10 次/分钟，或者把桶打空后
    /// 连带影响其它测试。这正是集成测试必须能覆盖 <see cref="TokenRateLimitOptions"/> 的原因。
    /// </para>
    /// </remarks>
    public class TokenRateLimitMiddleware(RequestDelegate next, IOptions<TokenRateLimitOptions> options)
    {
        private readonly PartitionedRateLimiter<HttpContext> _tokenLimiter =
            PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var settings = options.Value;
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = settings.PermitLimit,
                    Window = settings.Window,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/connect/token", StringComparison.OrdinalIgnoreCase))
            {
                using var lease = await _tokenLimiter.AcquireAsync(context);
                if (!lease.IsAcquired)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too Many Requests");
                    return;
                }
            }
            await next(context);
        }
    }
}