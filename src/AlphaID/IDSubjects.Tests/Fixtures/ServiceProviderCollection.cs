using Xunit;

namespace IDSubjectsTests.Fixtures;

[CollectionDefinition(nameof(ServiceProviderCollection))]
public class ServiceProviderCollection : ICollectionFixture<ServiceProviderFixture>
{
}
