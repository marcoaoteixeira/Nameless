namespace Nameless.Sample.WeatherForecast.Models;

public class Forecast {
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }
}
