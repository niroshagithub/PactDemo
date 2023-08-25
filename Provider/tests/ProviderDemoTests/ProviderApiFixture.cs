using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ProviderDemo;

namespace ProviderDemoTests;

public class ProviderApiFixture : IDisposable
{
    private readonly IHost _server;
    public Uri ServerUri { get; }

    public ProviderApiFixture()
    {
        ServerUri = new Uri("http://localhost:5170");
        _server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(ServerUri.ToString());
                webBuilder.UseStartup<TestStartup>();
            })
            .Build();
        _server.Start();
    }

    public void Dispose()
    {
        _server.Dispose();
    }
}