using Xunit;

namespace AlphaIdWebAPI.Tests;

[CollectionDefinition(nameof(TestServerCollection))]
public class TestServerCollection
    : ICollectionFixture<AlphaIdApiFactory>;