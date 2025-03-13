using System.Net;
using RadiusCore;
using RadiusCore.Dictionary;
using RadiusCore.Packet;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up Radius services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class RadiusServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Radius server services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddRadiusServer(this IServiceCollection services,
            Action<RadiusServerOptions>? setAction = null)
        {
            services.AddHostedService<RadiusServer>();

            services.AddTransient<IRadiusDictionary>(_ =>
            {
                return RadiusDictionary.LoadAsync(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\content\\radius.dictionary").GetAwaiter().GetResult();
            });
            services.AddTransient<IRadiusPacketParser, RadiusPacketParser>();
            services.AddTransient<TestPacketHandler>();
            services.AddTransient<IUdpClientFactory, UdpClientFactory>();
            services.AddTransient<IPacketHandlerRepository>(services1 =>
            {
                var packetHandler = services1.GetRequiredService<TestPacketHandler>();
                var repository = new PacketHandlerRepository();
                repository.AddPacketHandler(IPAddress.Parse("127.0.0.1"), packetHandler, "secret");
                return repository;
            });
            services.AddTransient<RadiusServer>();

            if (setAction != null)
                services.Configure(setAction);

            return services;
        }
    }
}
