namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

public class RemoteApiClientProvider(string baseUrl) : IApiClientProvider, IDisposable
{
    private readonly HttpClient _client = new() { BaseAddress = new Uri(baseUrl.TrimEnd('/')) };

    public HttpClient CreateClient() => _client;

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
