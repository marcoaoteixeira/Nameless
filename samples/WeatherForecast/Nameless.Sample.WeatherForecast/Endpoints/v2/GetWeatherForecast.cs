using Microsoft.AspNetCore.Mvc;
using Nameless.Sample.WeatherForecast.Dtos;
using Nameless.Sample.WeatherForecast.Services;
using Nameless.Web.Endpoints;

namespace Nameless.Sample.WeatherForecast.Endpoints.v2;

public class GetWeatherForecast : IEndpoint {
    private readonly IForecastService _forecastService;

    public GetWeatherForecast(IForecastService forecastService) {
        _forecastService = forecastService;
    }

    public void Configure(IEndpointBuilder builder) {
        builder
           .Get("/forecast", HandleAsync)
           .AllowAnonymous()
           .WithRoutePrefix("/weather")
           .WithTags("Information")
           .Produces<ForecastDto[]>()
           .ProducesProblem()
           .WithVersion(version: 2);
    }

    public Task<ForecastDto[]> HandleAsync([FromQuery] string? summary = null) {
        var forecasts = _forecastService.GetForecasts(summary).ToArray();

        return Task.FromResult(forecasts);
    }
}
