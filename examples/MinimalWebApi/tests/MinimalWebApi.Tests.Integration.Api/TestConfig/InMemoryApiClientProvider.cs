using Microsoft.AspNetCore.Mvc.Testing;

namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

public class InMemoryApiClientProvider : IApiClientProvider, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new TestApiFactory();

    public HttpClient CreateClient() => _factory.CreateClient();

    public void Dispose() => _factory.Dispose();
}
