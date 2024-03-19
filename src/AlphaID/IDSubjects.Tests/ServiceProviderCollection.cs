using Xunit;

namespace IdSubjects.Tests;

[CollectionDefinition(nameof(ServiceProviderCollection))]
public class ServiceProviderCollection : ICollectionFixture<ServiceProviderFixture>;
