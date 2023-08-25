using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ConsumerConsoleDemo;

public class ConsumerApiClient
{
    private readonly HttpClient _httpClient;

    public ConsumerApiClient(Uri baseAddress)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = baseAddress // Base URL of the API
        };

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<WeatherForecast[]> GetWeatherForecastsAsync()
    {
        using var response = await _httpClient.GetAsync("weatherforecast");
        response.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful

        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        return forecasts;
    }
}
