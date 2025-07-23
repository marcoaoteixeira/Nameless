using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.IdentityModel.Jwt;

/// <summary>
///     Extensions for <see cref="IHostApplicationBuilder"/> to register JSON Web Token provider.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers the JSON Web Token provider with the specified options.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type of the host application builder.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">
    ///     A configuration action to set up the <see cref="JsonWebTokenOptions"/>.
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterJsonWebTokenProvider<THostApplicationBuilder>(this THostApplicationBuilder self, Action<JsonWebTokenOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services.Configure(configure ?? (_ => { }));

        self.Services.TryAddTransient<IJsonWebTokenProvider, JsonWebTokenProvider>();

        return self;
    }
}
