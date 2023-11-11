using Xunit;

namespace AlphaIDWebAPITests;

[CollectionDefinition(nameof(TestServerCollection))]
public class TestServerCollection
    : ICollectionFixture<AlphaIdApiFactory>
{
}
