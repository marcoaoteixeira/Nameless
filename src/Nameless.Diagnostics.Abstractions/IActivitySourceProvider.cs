namespace Nameless.Diagnostics;

/// <summary>
///     Implements a provider for creating instances of
///     <see cref="IActivitySource"/>.
/// </summary>
public interface IActivitySourceProvider {
    /// <summary>
    ///     Creates a new <see cref="IActivitySource"/> instance
    /// </summary>
    /// <param name="name">
    ///     The activity source name.
    /// </param>
    /// <param name="version">
    ///     The activity source version.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IActivitySource"/>.
    /// </returns>
    IActivitySource Create(string name, string? version = null);
}