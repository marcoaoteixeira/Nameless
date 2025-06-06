using Microsoft.AspNetCore.Mvc;
using Nameless.Sample.WeatherForecast.Dtos;
using Nameless.Sample.WeatherForecast.Services;
using Nameless.Web.Endpoints;

namespace Nameless.Sample.WeatherForecast.Endpoints.v1;

public class GetWeatherForecast : IEndpoint {
    private readonly IForecastService _forecastService;

    public GetWeatherForecast(IForecastService forecastService) {
        _forecastService = forecastService;
    }

    public void Configure(IEndpointBuilder builder) {
        builder
           .Get("/forecast/{summary}", HandleAsync)
           .AllowAnonymous()
           .WithRouteSuffix("weather")
           .WithName($"{nameof(GetWeatherForecast)}_v1")
           .WithTags("Information")
           .Produces<ForecastDto[]>()
           .ProducesProblem()
           .WithVersion(version: 1);
    }

    public Task<ForecastDto[]> HandleAsync([FromRoute] string summary) {
        var forecasts = _forecastService.GetForecasts(summary).ToArray();

        return Task.FromResult(forecasts);
    }
}
