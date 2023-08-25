// See https://aka.ms/new-console-template for more information

using ConsumerConsoleDemo;

Console.WriteLine("Fetching Weather Forecasts");
var baseUri = new Uri("http://localhost:5170");
var consumerApiClient = new ConsumerApiClient(baseUri);
var weatherForecasts = await consumerApiClient.GetWeatherForecastsAsync();
foreach (var weatherForecast in weatherForecasts)
{
    Console.WriteLine(weatherForecast);    
}
