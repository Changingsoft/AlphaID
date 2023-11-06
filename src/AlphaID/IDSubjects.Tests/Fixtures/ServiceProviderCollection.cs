using Xunit;

namespace IDSubjects.Tests.Fixtures;

[CollectionDefinition(nameof(ServiceProviderCollection))]
public class ServiceProviderCollection : ICollectionFixture<ServiceProviderFixture>
{
}
