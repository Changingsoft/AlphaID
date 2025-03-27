using RadiusCore;
using RadiusCore.Dictionary;
using RadiusCore.Packet;
using System.Net;
using System.Net.Sockets;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Radius services in an <see cref="IServiceCollection" />.
/// </summary>
public static class RadiusServiceCollectionExtensions
{
    /// <summary>
    /// 添加RADIUS Server。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddRadiusServer(this IServiceCollection services,
        Action<RadiusServerOptions>? setAction = null)
    {
        services.AddRadiusCore();

        services.AddHostedService<RadiusServer>();

        if (setAction != null)
            services.Configure(setAction);

        return services;
    }

    /// <summary>
    /// 添加RADIUS核心服务。
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRadiusCore(this IServiceCollection services)
    {
        services.AddSingleton<IUdpClient>(_ => new DefaultUdpClient(new UdpClient(1812)));

        services.AddTransient(_ =>
        {
            return RadiusDictionary.LoadAsync(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\content\\radius.dictionary").GetAwaiter().GetResult();
        });
        services.AddTransient<RadiusRequestParser>();
        services.AddTransient<TestPacketHandler>();
        services.AddTransient<RadiusServer>();
        return services;
    }
}