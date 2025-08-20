using Microsoft.Extensions.Configuration;

namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

public class TestHttpClientFactory : IDisposable
{
    /// <summary>
    /// The default environment name if can't find an environment name variable (ie DOTNET_ENVIRONMENT, ASPNETCORE_ENVIRONMENT)
    /// @default: "development"
    /// </summary>
    public static string DefaultEnvironment { get; set; } = "development";

    /// <summary>
    /// The provider to use to create the HttpClient
    /// </summary>
    public IApiClientProvider Provider { get; }

    /// <summary>
    /// Create the appropriate HttpClient for either in memory or remote testing
    /// </summary>
    public HttpClient CreateClient() => Provider.CreateClient();

    public TestHttpClientFactory()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? DefaultEnvironment;

        // Load config
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var settings = config.GetSection(nameof(TestSettings)).Get<TestSettings>() ?? new TestSettings();

        if (settings.UseInMemory)
        {
            Provider = new InMemoryApiClientProvider();
        }
        else if (!string.IsNullOrWhiteSpace(settings.BaseUrl) && Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _))
        {
            Provider = new RemoteApiClientProvider(settings.BaseUrl);
        }
        else
        {
            throw new InvalidOperationException(
                $"Invalid {nameof(TestSettings)} in appsettings.{environment}.json. " +
                $"Must either have {nameof(TestSettings.UseInMemory)}=true or " +
                $"{nameof(TestSettings.BaseUrl)} set to a valid URL (eg 'https://my-api.test.example.com').");
        }
    }

    public void Dispose()
    {
        if (Provider is IDisposable disposable)
            disposable.Dispose();
    }
}
