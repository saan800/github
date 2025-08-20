namespace MinimalWebApi.Tests.Integration.Api.TestConfig;

public class TestSettings
{
    /// <summary>
    /// Run the tests in memory or against a deployed endpoint.
    /// @default: true
    /// </summary>
    /// <remarks>
    /// Must provide either UseInMemory=true or BaseUrl
    /// </remarks>
    public bool UseInMemory { get; set; } = true;

    /// <summary>
    /// The base URL of the deployed endpoint.
    /// Eg https://my-api.test.example.com
    /// </summary>
    /// <remarks>
    /// Must provide either UseInMemory=true or BaseUrl.
    /// BaseUrl must be a valid URL
    /// </remarks>
    public string? BaseUrl { get; set; }
}
