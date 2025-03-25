using Microsoft.Extensions.DependencyInjection;
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
            
            var context = new RadiusContext(new RadiusRequest(null, null, null), server,
                serviceProviderFixture.RootServiceProvider);
            
            var condition = new DateTimeCondition
            {
                Matrix = [0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff],
            };

            Assert.True(condition.TestCondition(context));
        }
    }
}
