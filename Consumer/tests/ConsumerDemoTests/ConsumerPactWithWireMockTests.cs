using System.Net;
using ConsumerConsoleDemo;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ConsumerDemoTests;

public class ConsumerPactWithWireMockTests
{
    [Fact]
    public async Task ItHandles200Response()
    {
        // Arrange
        var weatherForecasts = new List<object>()
        {
            new { date = DateTime.Parse("2023-01-01"), temperatureC = 32, summary = "Sunny" }
        };
        
        var server = WireMockServer.Start();
        var serverUrl = server.Url + "/";
        server
            .WithConsumer("Weather Forecast API Consumer Get")
            .WithProvider("Weather Forecast API")

            .Given(Request.Create()
                .UsingGet()
                .WithPath("/weatherforecast")
                .WithHeader("Accept", "application/json")
            )
            .WithTitle("A GET request to retrieve the weather forecasts")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBodyAsJson(weatherForecasts)
            );
        
        // Act
        var consumer = new ConsumerApiClient(new Uri(serverUrl));
        var result = await consumer.GetWeatherForecastsAsync();
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Summary.Should().Be("Sunny");
        result[0].TemperatureC.Should().Be(32);
        result[0].Date.Should().Be(DateTime.Parse("2023-01-01"));

        server.SavePact(Path.Combine("../../../../../../", "pacts"), "pact-get-weatherforecast-exists.json");
    }
    
    [Fact]
    public async Task ItHandles404Response()
    {
        // Arrange
        var server = WireMockServer.Start();
        var serverUrl = server.Url + "/";
        server
            .WithConsumer("Weather Forecast API Consumer Get")
            .WithProvider("Weather Forecast API")

            .Given(Request.Create()
                .UsingGet()
                .WithPath("/weatherforecast")
                .WithHeader("Accept", "application/json")
            )
            .WithTitle("A GET request to retrieve the weather forecasts")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
            );
        
        // Act
        var consumer = new ConsumerApiClient(new Uri(serverUrl));
        Func<Task> act = async () => await consumer.GetWeatherForecastsAsync();
            
        // Assert
        await act.Should().ThrowAsync<HttpRequestException>().Where(e => 
            e.Message.StartsWith("Response status code does not indicate success: 404 (Not Found)."));;
        
        server.SavePact(Path.Combine("../../../../../../", "pacts"), "pact-get-weatherforecast-doesnot-exist.json");
        
    }
}