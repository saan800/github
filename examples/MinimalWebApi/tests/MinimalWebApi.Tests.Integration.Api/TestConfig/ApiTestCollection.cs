namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

[CollectionDefinition(TestConfigConstants.Collections.Api)]
public class ApiTestCollection : ICollectionFixture<TestHttpClientFactory>
{
    // No code needed, just links the fixture to the collection
}
