using Microsoft.Extensions.Hosting;
using Nameless.Web.Discoverability;
using Nameless.Web.HealthChecks;
using Nameless.Web.OpenTelemetry;

namespace Nameless.Microservices.Aspire.Shared;

public static class HostApplicationBuilderExtension {
    /// <summary>
    /// Registers Aspire services and discovery.
    /// </summary>
    /// <typeparam name="TApplicationBuilder">Type of the application builder.</typeparam>
    /// <param name="self">The current <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <returns>The current <see cref="IHostApplicationBuilder"/> instance so other actions can be chained.</returns>
    public static TApplicationBuilder RegisterAspireServices<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder {
        return self.RegisterOpenTelemetry()
                   .RegisterHealthChecks()
                   .RegisterDiscoverability();
    }
}