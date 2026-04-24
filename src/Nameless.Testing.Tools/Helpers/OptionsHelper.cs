using Microsoft.Extensions.Options;
using Nameless.Helpers;

namespace Nameless.Testing.Tools.Helpers;

/// <summary>
///     Simple class to help with <see cref="IOptions{TOptions}"/>
/// </summary>
public static class OptionsHelper {
    /// <summary>
    ///     Creates a <see cref="IOptions{TOptions}"/> instance for given
    ///     configure delegate.
    /// </summary>
    /// <typeparam name="TOptions">
    ///     Type of the options.
    /// </typeparam>
    /// <param name="configure">
    ///     The configure delegate.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IOptions{TOptions}"/>.
    /// </returns>
    public static IOptions<TOptions> Create<TOptions>(Action<TOptions>? configure = null)
        where TOptions : class, new() {
        return Options.Create(
            ActionHelper.FromDelegate(configure)
        );
    }
}