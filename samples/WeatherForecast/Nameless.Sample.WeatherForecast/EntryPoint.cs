using Nameless.Sample.WeatherForecast.Configs;
using Nameless.Web.Endpoints;

namespace Nameless.Sample.WeatherForecast;

public static class EntryPoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
               .AddAuthorization()
               .RegisterApplicationServices()
               .RegisterMinimalEndpoints(configure => {
                   configure.Assemblies = [typeof(EntryPoint).Assembly];
               });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseRouting()
           .UseAuthorization()
           .UseMinimalEndpoints();

        app.Run();
    }
}
