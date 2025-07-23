using Microsoft.AspNetCore.Mvc;
using Nameless.Sample.WeatherForecast.Services;
using Nameless.Web.Endpoints;

namespace Nameless.Sample.WeatherForecast.Endpoints.v1;

public class GetWeatherForecast : IEndpoint {
    private readonly IForecastService _forecastService;

    public GetWeatherForecast(IForecastService forecastService) {
        _forecastService = forecastService;
    }

    public void Configure(IEndpointDescriptor descriptor) {
        descriptor
           .Get("/forecast", HandleAsync)
           .AllowAnonymous()
           .WithTags("Information")
           .Produces<OkResult>()
           .ProducesProblem()
           .WithVersion(version: 1);
    }

    public Task<IResult> HandleAsync([FromQuery] string? summary = null) {
        var forecasts = _forecastService.GetForecasts(summary).ToArray();

        IResult ok = TypedResults.Ok(new {
            Version = 1,
            Result = forecasts
        });

        return Task.FromResult(ok);
    }
}
