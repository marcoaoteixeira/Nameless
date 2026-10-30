using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;
using Nameless.Web.Correlation;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers the HTTP request correlation ID services and uses their middleware.
/// </summary>
public class HttpRequestCorrelationWebHostFeature : HostFeature, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.ConfigureOptions<HttpRequestCorrelationOptions>(
            builder.Configuration
        );
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseMiddleware<HttpRequestCorrelationMiddleware>(
            app.Services.GetOptions<HttpRequestCorrelationOptions>()
        );
    }
}
