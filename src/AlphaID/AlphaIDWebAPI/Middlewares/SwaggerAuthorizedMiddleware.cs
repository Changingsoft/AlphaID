using Microsoft.AspNetCore.Authentication;

namespace AlphaIDWebAPI.Middlewares;

/// <summary>
/// 为swagger提供授权访问的中间件。详见<see href="https://github.com/domaindrivendev/Swashbuckle.WebApi/issues/384#issuecomment-272310182"/>
/// </summary>
public class SwaggerAuthorizedMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public SwaggerAuthorizedMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/docs"))
        {
            if (!(context.User.Identity != null && context.User.Identity.IsAuthenticated))
            {
                await context.ChallengeAsync();
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await this._next.Invoke(context);
    }
}