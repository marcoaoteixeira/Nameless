using Microsoft.AspNetCore.Builder;
using Nameless.Hosting;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Uses the HTTPS redirection middleware.
/// </summary>
public class HttpsRedirectionWebHostFeature : HostFeature, IWebHostFeature {
    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseHttpsRedirection();
    }
}
