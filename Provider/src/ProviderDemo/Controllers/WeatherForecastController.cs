using Microsoft.AspNetCore.Mvc;
using ProviderDemo.Repositories;

namespace ProviderDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, 
        IWeatherForecastRepository weatherForecastRepository)
    {
        _logger = logger;
        _weatherForecastRepository = weatherForecastRepository;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        var forecasts =  _weatherForecastRepository.GetWeatherForecasts();
        return forecasts.Any() ? Ok(forecasts) : NotFound();
    }
}
