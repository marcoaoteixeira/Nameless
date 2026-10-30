using Microsoft.Extensions.Hosting;
using Nameless.Contracts;

namespace Nameless.Hosting;

/// <summary>
///     Represents a feature of the host, like logging, that
///     <see cref="HostFactory"/> registers and uses when building the
///     application.
/// </summary>
public abstract class HostFeature {
    /// <summary>
    ///     Gets the feature name.
    /// </summary>
    public virtual string Name => GetType().Name;

    /// <summary>
    ///     Registers the services required by the feature.
    /// </summary>
    /// <param name="builder">
    ///     The <see cref="IHostApplicationBuilder"/> being configured.
    /// </param>
    /// <param name="context">
    ///     The <see cref="HostFeatureContext"/> shared by every feature.
    /// </param>
    public virtual void Register(IHostApplicationBuilder builder, HostFeatureContext context) { }
}

/// <summary>
///     Represents a <see cref="HostFeature"/> that can be configured
///     through a <typeparamref name="TOptions"/> object.
/// </summary>
/// <typeparam name="TOptions">
///     Type of the object used to configure the feature.
/// </typeparam>
public abstract class HostFeature<TOptions> : HostFeature {
    /// <summary>
    ///     Gets the composition of every delegate passed to
    ///     <see cref="Configure"/>, or <see langword="null"/> when it was
    ///     never called.
    /// </summary>
    /// <remarks>
    ///     Features must apply their own defaults before invoking it, so the
    ///     user configuration always extends the defaults.
    /// </remarks>
    protected Action<TOptions>? ConfigureFeature { get; private set; }

    /// <summary>
    ///     Adds a configuration step to the feature.
    /// </summary>
    /// <remarks>
    ///     Every call adds to the previous ones; steps run in call order.
    /// </remarks>
    /// <param name="configure">
    ///     The configuration delegate.
    /// </param>
    /// <returns>
    ///     The current feature so other actions can be chained.
    /// </returns>
    public HostFeature<TOptions> Configure(Action<TOptions> configure) {
        ConfigureFeature += Throws.When.Null(configure);

        return this;
    }
}