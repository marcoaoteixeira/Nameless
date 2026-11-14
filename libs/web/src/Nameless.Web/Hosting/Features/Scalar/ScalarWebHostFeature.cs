using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Scalar.AspNetCore;

namespace Nameless.Web.Hosting.Features.Scalar;

/// <summary>
///     Maps the Scalar API reference.
/// </summary>
public class ScalarWebHostFeature : HostFeature<ScalarFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        var combine = Delegate.Combine(ConfigureDefaults, options.ConfigureScalarOptions);

        app.MapScalarApiReference((Action<ScalarOptions, HttpContext>)combine);
    }

    private static void ConfigureDefaults(ScalarOptions options, HttpContext ctx) {
        var applicationName = ctx.RequestServices.GetService<IHostEnvironment>()?.ApplicationName ?? "Application Name";

        options
            .WithTitle(applicationName)
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
            .SortOperationsByMethod();
    }
}
