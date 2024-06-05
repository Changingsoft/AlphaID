namespace AdminWebApp.Tests;

[CollectionDefinition(nameof(TestServerCollection))]
public class TestServerCollection : ICollectionFixture<AdminWebAppFactory>;