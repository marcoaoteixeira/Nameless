using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Identity.Infrastructure;

/// <summary>
///     Extensions for <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers the JSON Web Token services.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type of the host application builder.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterJsonWebTokenServices<THostApplicationBuilder>(this THostApplicationBuilder self)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services.TryAddTransient<IJsonWebTokenService, JsonWebTokenService>();

        return self;
    }
}
