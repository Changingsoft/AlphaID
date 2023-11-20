using Xunit;

namespace IdSubjects.Tests.Fixtures;

[CollectionDefinition(nameof(ServiceProviderCollection))]
public class ServiceProviderCollection : ICollectionFixture<ServiceProviderFixture>
{
}
