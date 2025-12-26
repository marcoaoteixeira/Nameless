using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Correlation;

/// <summary>
///     <see cref="IHostApplicationBuilder"/> extension methods.
/// </summary>
public static class ApplicationBuilderExtensions {
    /// <summary>
    ///     Configures the correlation middleware in the application.
    /// </summary>
    /// <typeparam name="TApplicationBuilder">
    ///     Type of the application builder.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="TApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">
    ///     A callback to configure the <see cref="HttpContextCorrelationMiddleware"/>.
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="TApplicationBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public static TApplicationBuilder UseHttpContextCorrelation<TApplicationBuilder>(this TApplicationBuilder self,
        Action<HttpContextCorrelationOptions>? configure = null)
        where TApplicationBuilder : IApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new HttpContextCorrelationOptions();

        innerConfigure(options);

        self.UseMiddleware<HttpContextCorrelationMiddleware>(options);

        return self;
    }
}