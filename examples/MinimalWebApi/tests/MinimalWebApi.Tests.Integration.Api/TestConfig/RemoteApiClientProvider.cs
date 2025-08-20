namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

public class RemoteApiClientProvider(string baseUrl) : IApiClientProvider
{
    private readonly string _baseUrl = baseUrl.TrimEnd('/');

    public HttpClient CreateClient() => new() { BaseAddress = new Uri(_baseUrl) };
}
