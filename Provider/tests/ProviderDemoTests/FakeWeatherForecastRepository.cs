using ProviderDemo;
using ProviderDemo.Repositories;

namespace ProviderDemoTests;

public class FakeWeatherForecastRepository :IWeatherForecastRepository
{
    public static List<WeatherForecast> WeatherForecasts = new();
    public IEnumerable<WeatherForecast> GetWeatherForecasts()
    {
        return WeatherForecasts;
    }
}