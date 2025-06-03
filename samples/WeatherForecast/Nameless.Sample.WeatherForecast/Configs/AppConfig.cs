using Nameless.Sample.WeatherForecast.Repositories;
using Nameless.Sample.WeatherForecast.Services;

namespace Nameless.Sample.WeatherForecast.Configs;

public static class AppConfig {
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection self) {
        return self.AddSingleton<IForecastService, ForecastService>()
                   .AddSingleton<IForecastRepository, ForecastRepository>();
    }
}
