using System.Net;
using ConsumerConsoleDemo;
using FluentAssertions;
using PactNet;
using PactNet.Matchers;
using Xunit;

namespace ConsumerDemoTests;

public class ConsumerPactTests
{
    private readonly IPactBuilderV4 _pactBuilder;

    public ConsumerPactTests()
    {
        var pact = Pact.V4("WeatherForecast Consumer", "WeatherForecast API", new PactConfig
        {
            PactDir = $"{Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName}{Path.DirectorySeparatorChar}pacts"
        });

        // Initialize Rust backend
        _pactBuilder = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task ItHandles200Response()
    {
        // Arrange
        var weatherForecasts = new List<object>()
        {
            new { date = DateTime.Parse("2023-01-01"), temperatureC = 32, summary = "Sunny" }
        };
        _pactBuilder.UponReceiving("A GET request to get weather forecasts")
                        .Given("weather forecasts exist")
                        .WithRequest(HttpMethod.Get, "/weatherforecast")
                    .WillRespond()
                        .WithStatus(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json; charset=utf-8")
                        .WithJsonBody(Match.MinType(weatherForecasts[0],1));

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            // Act
            var consumer = new ConsumerApiClient(ctx.MockServerUri);
            var result = await consumer.GetWeatherForecastsAsync();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Summary.Should().Be("Sunny");
            result[0].TemperatureC.Should().Be(32);
            result[0].Date.Should().Be(DateTime.Parse("2023-01-01"));
        });
    }
    
    [Fact]
    public async Task ItHandles404Response()
    {
        // Arrange
        _pactBuilder.UponReceiving("A GET request to get weather forecasts")
            .Given("weather forecasts doesn't exist")
            .WithRequest(HttpMethod.Get, "/weatherforecast")
            .WillRespond()
            .WithStatus(HttpStatusCode.NotFound)
           ;

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            // Act
            var consumer = new ConsumerApiClient(ctx.MockServerUri);
            Func<Task> act = async () => await consumer.GetWeatherForecastsAsync();
            
            // Assert
            await act.Should().ThrowAsync<HttpRequestException>().Where(e => 
                e.Message.StartsWith("Response status code does not indicate success: 404 (Not Found)."));;
        });
    }
}