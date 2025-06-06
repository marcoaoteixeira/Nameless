namespace Nameless.Sample.WeatherForecast.Dtos;

public record ForecastDto {
    public DateOnly Date { get; init; }

    public int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public required string Summary { get; init; }
}
