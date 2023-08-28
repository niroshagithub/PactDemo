using Swashbuckle.AspNetCore.Filters;

namespace ProviderDemo;

public class WeatherForecastExample : IMultipleExamplesProvider<WeatherForecast>
{
    public IEnumerable<SwaggerExample<WeatherForecast>> GetExamples()
    {
        yield return SwaggerExample.Create(
        "Example 1", new WeatherForecast()
        {
            Date = DateTime.Now,
            Summary = "Chilly",
            TemperatureC = 15
        });
        yield return SwaggerExample.Create(
            "Example 2", new WeatherForecast()
            {
                Date = DateTime.Now,
                Summary = "Warm",
                TemperatureC = 30
            });
    }
}
