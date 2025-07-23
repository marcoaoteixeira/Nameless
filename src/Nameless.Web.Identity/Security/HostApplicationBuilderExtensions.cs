using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web.Identity.Security;

/// <summary>
///     Extensions for <see cref="IHostApplicationBuilder"/> to register user refresh token services.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers the user refresh token service with the specified options.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type of the host application builder.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">
    ///     A configuration action to set up the
    ///     <see cref="UserRefreshTokenOptions"/>.
    /// </param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterUserRefreshToken<THostApplicationBuilder>(this THostApplicationBuilder self, Action<UserRefreshTokenOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services.Configure(configure ?? (_ => { }));

        self.Services.TryAddTransient<IUserRefreshTokenManager, UserRefreshTokenManager>();

        return self;
    }
}
