namespace AlphaIdWebAPI.Middlewares;

/// <summary>
/// 
/// </summary>
public static class SwaggerAuthorizeExtensions
{
    /// <summary>
    /// Protect Swagger via authentication and authorization.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SwaggerAuthorizedMiddleware>();
    }
}