using Microsoft.Extensions.DependencyInjection;

namespace RadiusCore.Tests
{
    public class ServiceProviderFixture
    {
        public ServiceProviderFixture()
        {
            var services = new ServiceCollection();

            services.AddOptions();
            services.AddLogging();
            services.AddRadiusCore();

            //将部分组件替换为测试组件
            services.AddTransient<IUdpClientFactory, MockUdpClientFactory>(services =>
            {
                return new MockUdpClientFactory(new MockUdpClient());
            });

            RootServiceProvider = services.BuildServiceProvider();
            ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public IServiceProvider RootServiceProvider { get; }

        public IServiceScopeFactory ServiceScopeFactory { get; }
    }


}
