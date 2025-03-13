using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Tests
{
    public class ServiceProviderFixture
    {
        public ServiceProviderFixture()
        {
            var services = new ServiceCollection();

            services.AddRadiusServer();

            RootServiceProvider = services.BuildServiceProvider();
            ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        public IServiceProvider RootServiceProvider { get; }

        public IServiceScopeFactory ServiceScopeFactory { get; }
    }


}
