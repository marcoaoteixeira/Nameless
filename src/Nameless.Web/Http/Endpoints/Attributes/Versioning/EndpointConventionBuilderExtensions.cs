using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

/// <summary>
///     <see cref="RouteHandlerBuilder"/> extension methods.
/// </summary>
public static class EndpointConventionBuilderExtensions {
    /// <summary>
    ///     
    /// </summary>
    /// <param name="builder">
    ///     The current <see cref="RouteHandlerBuilder"/> instance.
    /// </param>
    /// <param name="sunsetDate">
    ///     The date and time after which the endpoint is considered
    ///     sunset.
    /// </param>
    /// <param name="link">
    ///     The link pointing to migration documentation.
    /// </param>
    /// <returns>
    ///     The current <see cref="RouteHandlerBuilder"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public static RouteHandlerBuilder WithSunset(this RouteHandlerBuilder builder, DateTimeOffset sunsetDate, string? link = null) {
        return builder.WithMetadata(new SunsetMetadata(sunsetDate) { Link = link })
                      .AddEndpointFilter<SunsetFilter>();
    }
}