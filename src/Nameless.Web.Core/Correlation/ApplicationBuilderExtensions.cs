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
    /// <returns>
    ///     The current <typeparamref name="TApplicationBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public static TApplicationBuilder UseCorrelationMiddleware<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IApplicationBuilder {
        self.UseMiddleware<CorrelationMiddleware>();

        return self;
    }
}
