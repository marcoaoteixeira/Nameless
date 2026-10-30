using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers antiforgery services, using the <c>X-CSRF-TOKEN</c> header
///     and an HTTP-only, secure-only cookie of the same name, and uses the
///     antiforgery middleware.
/// </summary>
public class AntiforgeryWebHostFeature : HostFeature<AntiforgeryOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.AddAntiforgery(ConfigureFeature ?? ConfigureDefaultAntiforgery);
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseAntiforgery();
    }

    private static void ConfigureDefaultAntiforgery(AntiforgeryOptions opts) {
        opts.HeaderName = "X-CSRF-TOKEN";
        opts.Cookie.Name = "X-CSRF-TOKEN";
        opts.Cookie.HttpOnly = true;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
}
