using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

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
    private readonly IConfiguration _config;
    public ProviderApiTests(ITestOutputHelper output)
    {
        _server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(ProviderUri.ToString());
                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddUserSecrets<Program>(); // Add user secrets configuration
                });
                webBuilder.UseStartup<TestStartup>();
            })
            .Build();
        _config =_server.Services.GetRequiredService<IConfiguration>();;

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
        
        var pactUriString = _config["PactBroker:PactUri"];
        var pactBrokerToken = _config["PactBroker::PactBrokerToken"];
        if (pactUriString == null || pactBrokerToken == null)
        {
            _verifier
                .WithHttpEndpoint(ProviderUri)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(ProviderUri, "/provider-states"))
                .Verify();

            return;
        }

        var pactUri = new Uri(pactUriString);

        // Assert
        _verifier
            .WithHttpEndpoint(ProviderUri)
            .WithUriSource(pactUri, options =>
            {
                options.TokenAuthentication(pactBrokerToken);
                options.PublishResults(true, "1.0.1", settings => settings.ProviderBranch("master")
                    .ProviderTags("basictest")
                    .BuildUri(pactUri));
            })
            //.WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(ProviderUri, "/provider-states"))
            .Verify();
    }
}