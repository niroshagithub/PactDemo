using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProviderDemo;

namespace ProviderDemoTests;

public class ProviderStateMiddleware
{
     private static readonly JsonSerializerOptions? Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _providerStates;
        private readonly RequestDelegate _next;
    

        /// <summary>
        /// Initialises a new instance of the <see cref="ProviderStateMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="orders">Orders repository for actioning provider state requests</param>
        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            
            _providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["weather forecasts exist"] = EnsureDataExistsAsync,
                ["weather forecasts doesn't exist"] = EnsureDataDoesNotExistsAsync,
            };
        }

        /// <summary>
        /// Ensure an event exists
        /// </summary>
        /// <param name="parameters">Event parameters</param>
        /// <returns>Awaitable</returns>
        private Task EnsureDataExistsAsync(IDictionary<string, object> parameters)
        {
            FakeWeatherForecastRepository.WeatherForecasts = new List<WeatherForecast>()
            {
                new WeatherForecast
                {
                    Date = DateTime.Parse("2023-01-01"),
                    TemperatureC = 32,
                    Summary = "Sunny"
                }
            };
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Ensure an event exists
        /// </summary>
        /// <param name="parameters">Event parameters</param>
        /// <returns>Awaitable</returns>
        private Task EnsureDataDoesNotExistsAsync(IDictionary<string, object> parameters)
        {
            FakeWeatherForecastRepository.WeatherForecasts = new List<WeatherForecast>();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="context">Request context</param>
        /// <returns>Awaitable</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/provider-states"))
            {
                await HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(string.Empty);
            }
            else
            {
                await _next(context);
            }
        }
        
        private async Task HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (string.Equals(context.Request.Method, HttpMethod.Post.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                string jsonRequestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null && !string.IsNullOrEmpty(providerState.State))
                {
                    await _providerStates[providerState.State].Invoke(providerState.Params);
                }
            }
        }
}