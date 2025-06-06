using Nameless.Sample.WeatherForecast.Dtos;

namespace Nameless.Sample.WeatherForecast.Services;

public interface IForecastService {
    IEnumerable<ForecastDto> GetForecasts(string? summary = null);
}