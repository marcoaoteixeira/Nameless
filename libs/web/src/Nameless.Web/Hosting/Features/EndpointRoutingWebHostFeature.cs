using Microsoft.AspNetCore.Builder;
using Nameless.Hosting;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Uses the endpoint routing middleware.
/// </summary>
public class EndpointRoutingWebHostFeature : HostFeature, IWebHostFeature {
    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseRouting();
    }
}
