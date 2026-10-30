using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using System.Reflection;

namespace Nameless.Hosting;

/// <summary>
///     Read-only data shared by every <see cref="HostFeature"/> while the
///     host is being built.
/// </summary>
public sealed class HostFeatureContext {
    /// <summary>
    ///     Gets the assemblies to scan for types.
    /// </summary>
    public IReadOnlyList<Assembly> Assemblies { get; }

    /// <summary>
    ///     Gets the startup logger.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Every startup log entry uses the same category, the full name
    ///         of <see cref="HostFactory"/>, so a single filter shows
    ///         everything that happened while building the web host.
    ///     </para>
    ///     <para>
    ///         The logger is disposed once the host is built. Use it only
    ///         inside <see cref="HostFeature.Register"/>; don't capture it
    ///         in callbacks that run later (options, lifetime events), since
    ///         those entries are lost. Resolve an <c>ILogger&lt;T&gt;</c>
    ///         from the application services instead.
    ///     </para>
    /// </remarks>
    public ILogger Logger { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="HostFeatureContext"/>.
    /// </summary>
    /// <param name="assemblies">
    ///     The assemblies to scan for types.
    /// </param>
    /// <param name="logger">
    ///     The startup logger.
    /// </param>
    public HostFeatureContext(IReadOnlyList<Assembly> assemblies, ILogger logger) {
        // Copy, so a feature cannot change the list seen by the others.
        Assemblies = [.. Throws.When.Null(assemblies)];
        Logger = Throws.When.Null(logger);
    }
}
