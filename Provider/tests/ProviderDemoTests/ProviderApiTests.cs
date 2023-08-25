using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;

namespace ProviderDemoTests;

public class ProviderApiTests : IDisposable
{
    private static readonly Uri ProviderUri = new Uri("http://localhost:5170");
    private readonly IHost _server;
    private readonly PactVerifier _verifier;

    public ProviderApiTests(ITestOutputHelper output)
    {
        _server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(ProviderUri.ToString());
                webBuilder.UseStartup<TestStartup>();
            })
            .Build();

        _server.Start();
            
        _verifier = new PactVerifier("WeatherForecast API", new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new List<IOutput>
            {
                new XunitOutput(output)
            }
        });
    }
    
    public void Dispose()
    {
        _server.Dispose();
        _verifier.Dispose();
    }

    [Fact]
    public void EnsureProviderApiHonoursPactWithConsumer()
    {
        // Arrange
        var pactPath = Path.Combine("..",
            "..",
            "..",
            "..",
            "..",
            "..",
            "pacts",
            "WeatherForecast Consumer-WeatherForecast API.json");

        // Assert
        _verifier
            .WithHttpEndpoint(ProviderUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(ProviderUri, "/provider-states"))
            .Verify();
    }
}