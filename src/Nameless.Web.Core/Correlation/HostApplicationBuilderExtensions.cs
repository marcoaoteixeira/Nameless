using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Correlation;

/// <summary>
///     <see cref="IHostApplicationBuilder"/> extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Configures the correlation accessor in the host application
    ///     builder.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type of the host application builder.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">
    ///     A callback to configure the <see cref="CorrelationOptions"/>.    
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterCorrelationAccessor<THostApplicationBuilder>(this THostApplicationBuilder self, Action<CorrelationOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .Configure(configure ?? (_ => { }))

            // Ensure IHttpContextAccessor is available since
            // ICorrelationAccessor depends on it. There is no
            // harm in calling this multiple times because
            // it is only added if it is not already registered.
            .AddHttpContextAccessor()

            .AddSingleton<ICorrelationAccessor, CorrelationAccessor>();

        return self;
    }
}
