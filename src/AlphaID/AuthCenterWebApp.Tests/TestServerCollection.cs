namespace AuthCenterWebApp.Tests;

[CollectionDefinition(nameof(TestServerCollection))]
public class TestServerCollection : ICollectionFixture<AuthCenterWebAppFactory>;