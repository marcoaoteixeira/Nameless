using Microsoft.AspNetCore.Authentication.JwtBearer;
using Nameless.Sample.WeatherForecast.Configs;
using Nameless.Web.Endpoints;
using Nameless.Web.OpenApi;
using Scalar.AspNetCore;

namespace Nameless.Sample.WeatherForecast;

public static class EntryPoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
               .AddAuthorization()
               .AddOpenApi("v1", options => {
                   options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
               })
              .AddOpenApi("v2", options => {
                  options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
              })
              .AddAuthentication(options => {
                  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
             .AddJwtBearer();

        builder.Services
               .RegisterApplicationServices()
               .RegisterMinimalEndpoints(configure => {
                   configure.Assemblies = [typeof(EntryPoint).Assembly];
               });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
            app.MapScalarApiReference(options => {
                options.Title = "Weather Forecast App";
                options.Theme = ScalarTheme.BluePlanet;
                options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting()
           .UseAuthorization()
           .UseMinimalEndpoints();

        app.Run();
    }
}
