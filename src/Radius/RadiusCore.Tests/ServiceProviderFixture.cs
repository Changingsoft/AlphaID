using Microsoft.Extensions.DependencyInjection;

namespace RadiusCore.Tests
{
    public class ServiceProviderFixture
    {
        public ServiceProviderFixture()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            services.AddRadiusServer();

            //将部分组件替换为测试组件
            services.AddTransient<IUdpClientFactory, UdpClientMockFactory>(services =>
            {
                return new UdpClientMockFactory(new UdpClientMock());
            });

            RootServiceProvider = services.BuildServiceProvider();
            ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public IServiceProvider RootServiceProvider { get; }

        public IServiceScopeFactory ServiceScopeFactory { get; }
    }


}
