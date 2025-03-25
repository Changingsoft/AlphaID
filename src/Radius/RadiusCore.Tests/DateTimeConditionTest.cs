using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace RadiusCore.Tests
{
    [Collection(nameof(ServiceProviderCollection))]
    public class DateTimeConditionTest(ServiceProviderFixture serviceProviderFixture)
    {
        [Fact]
        public void Test()
        {
            var server = serviceProviderFixture.RootServiceProvider.GetRequiredService<RadiusServer>();
            server.TimeProvider = new FrozenTimeProvider(new DateTimeOffset(2025, 1, 1, 2, 15, 06, TimeSpan.FromHours(8)));
            
            var context = new RadiusContext(new RadiusRequest(PacketCode.AccessRequest, 0, new byte[16], [], new(IPAddress.Loopback, 1812)), server,
                serviceProviderFixture.RootServiceProvider);
            
            var condition = new DateTimeCondition
            {
                Matrix = [0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff],
            };

            Assert.True(condition.TestCondition(context));
        }
    }
}
