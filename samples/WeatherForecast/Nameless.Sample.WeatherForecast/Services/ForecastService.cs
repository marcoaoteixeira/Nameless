using Nameless.Sample.WeatherForecast.Dtos;
using Nameless.Sample.WeatherForecast.Repositories;

namespace Nameless.Sample.WeatherForecast.Services;

public class ForecastService : IForecastService {
    private readonly IForecastRepository _repository;

    public ForecastService(IForecastRepository repository) {
        _repository = repository;
    }

    public IEnumerable<ForecastDto> GetForecasts(string? summary = null) {
        var query = _repository.Query();

        if (!string.IsNullOrWhiteSpace(summary)) {
            query = query.Where(forecast => forecast.Summary == summary);
        }

        return query.Select(item => new ForecastDto {
            Date = item.Date,
            Summary = item.Summary ?? "Unknown",
            TemperatureC = item.TemperatureC
        });
    }
}