namespace ConsumerConsoleDemo;

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
    public int TemperatureF { get; set; }
    public override string ToString()
    {
        return $"Date: {Date:o}, TemperatureC: {TemperatureC}, TemperatureF: {TemperatureF}, Summary: {Summary}";
    }
}