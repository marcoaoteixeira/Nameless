using Nameless.Sample.WeatherForecast.Models;

namespace Nameless.Sample.WeatherForecast.Repositories;

public interface IForecastRepository {
    IQueryable<Forecast> Query();
}