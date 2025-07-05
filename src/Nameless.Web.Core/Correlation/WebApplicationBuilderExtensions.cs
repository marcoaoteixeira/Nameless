using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Correlation;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <summary>
    ///     Configures the correlation accessor in the host application
    ///     builder.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">
    ///     A callback to configure the <see cref="CorrelationOptions"/>.    
    /// </param>
    /// <returns>
    ///     The current <see cref="WebApplicationBuilder"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public static WebApplicationBuilder RegisterCorrelationAccessor(this WebApplicationBuilder self, Action<CorrelationOptions>? configure = null) {
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
