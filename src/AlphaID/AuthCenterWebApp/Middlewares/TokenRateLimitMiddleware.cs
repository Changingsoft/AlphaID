using System.Threading.RateLimiting;

namespace AuthCenterWebApp.Middlewares
{
    /// <summary>
    /// 针对 /connect/token 的基于IP的速率限制中间件。
    /// </summary>
    public class TokenRateLimitMiddleware(RequestDelegate next)
    {
        private static readonly PartitionedRateLimiter<HttpContext> s_tokenLimiter =
            PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/connect/token", StringComparison.OrdinalIgnoreCase))
            {
                using var lease = await s_tokenLimiter.AcquireAsync(context);
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