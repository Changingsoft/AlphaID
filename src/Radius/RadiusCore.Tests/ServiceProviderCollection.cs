using Xunit;

namespace RadiusCore.Tests
{
    [CollectionDefinition(nameof(ServiceProviderCollection))]
    public class ServiceProviderCollection:ICollectionFixture<ServiceProviderFixture>
    {
    }
}
