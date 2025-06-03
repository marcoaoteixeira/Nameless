using Bogus;
using Nameless.Sample.WeatherForecast.Models;

namespace Nameless.Sample.WeatherForecast.Repositories;

public class ForecastRepository : IForecastRepository {
    private static readonly string[] Summaries = [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];
    private static readonly List<Forecast> Cache = [];

    static ForecastRepository() {
        Cache = new Faker<Forecast>()
               .StrictMode(true)
               .RuleFor(item => item.Date, value => DateOnly.FromDateTime(value.Date.Future()))
               .RuleFor(item => item.TemperatureC, value => value.Random.Int(-20, 55))
               .RuleFor(item => item.Summary, value => value.PickRandom(Summaries))
               .Generate(50);
    }

    public IQueryable<Forecast> Query() {
        return Cache.AsQueryable();
    }
}