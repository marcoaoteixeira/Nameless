using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Scalar;
using Scalar.AspNetCore;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Maps the Scalar API reference.
/// </summary>
public class ScalarWebHostFeature : HostFeature<ScalarRegistration>, IWebHostFeature {
    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        var environment = app.Environment;

        app.MapScalarApiReference((scalar, httpContext) => {
            ConfigureDefaultScalar(scalar, environment);
            options.ConfigureScalarOptions?.Invoke(scalar, httpContext);
        });
    }

    private static void ConfigureDefaultScalar(ScalarOptions options, IHostEnvironment env) {
        options
            .WithTitle(env.ApplicationName)
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
            .SortOperationsByMethod();
    }
}
