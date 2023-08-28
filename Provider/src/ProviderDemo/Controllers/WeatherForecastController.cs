using Microsoft.AspNetCore.Mvc;
using ProviderDemo.Repositories;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ProviderDemo.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
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

    /// <summary>
    /// Get a list with all "<see cref="WeatherForecast"/>" items.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /WeatherForecast
    ///
    /// </remarks>
    /// <response code="200">Returns the list of weather forecasts</response>
    [HttpGet(Name = "GetWeatherForecast")]
    [SwaggerResponseExample(200, typeof(WeatherForecastExample))]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<WeatherForecast>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Get()
    {
        var forecasts =  _weatherForecastRepository.GetWeatherForecasts();
        return forecasts.Any() ? Ok(forecasts) : NotFound();
    }
}
