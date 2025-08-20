namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

public interface IApiClientProvider
{
    HttpClient CreateClient();
}
