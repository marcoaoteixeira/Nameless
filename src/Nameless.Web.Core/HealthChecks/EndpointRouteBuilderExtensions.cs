using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Nameless.Web.HealthChecks.Reporting;

namespace Nameless.Web.HealthChecks;

/// <summary>
///     <see cref="IEndpointRouteBuilder"/> extension methods for configure
///     health checks endpoints.
/// </summary>
public static class EndpointRouteBuilderExtensions {
    /// <summary>
    ///     Uses health checks endpoints in the application.
    /// </summary>
    /// <typeparam name="TEndpointRouteBuilder">
    ///     Type of the endpoint builder.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="TEndpointRouteBuilder"/>.
    /// </param>
    /// <param name="environment">The environment.</param>
    /// <returns>
    ///     The current <typeparamref name="TEndpointRouteBuilder"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <remarks>
    ///     Call this method after <c>UseRouting</c>.
    /// </remarks>
    public static TEndpointRouteBuilder UseHealthChecks<TEndpointRouteBuilder>(this TEndpointRouteBuilder self, IHostEnvironment environment)
        where TEndpointRouteBuilder : IEndpointRouteBuilder {
        // Adding health checks endpoints to applications in non-development
        // environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before
        // enabling these endpoints in non-development environments.
        if (!environment.IsDevelopment()) {
            return self;
        }

        // All health checks must pass for app to be considered ready
        // to accept traffic after starting
        self.MapHealthChecks("/health", new HealthCheckOptions {
            ResponseWriter = JsonReportWriter.WriteAsync
        });

        // Only health checks tagged with the "live" tag must pass for
        // app to be considered alive
        self.MapHealthChecks("/alive", new HealthCheckOptions {
            Predicate = registration => registration.Tags.Contains("live")
        });

        return self;
    }
}
