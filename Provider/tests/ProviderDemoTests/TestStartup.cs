using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProviderDemo;
using ProviderDemo.Repositories;

namespace ProviderDemoTests;

public class TestStartup
{
    private readonly Startup _inner;

    public TestStartup(IConfiguration configuration)
    {
        _inner = new Startup(configuration);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(Startup))!);
        _inner.ConfigureServices(services);
        services.AddScoped<IWeatherForecastRepository, FakeWeatherForecastRepository>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ProviderStateMiddleware>();

        _inner.Configure(app, env);
    }
}