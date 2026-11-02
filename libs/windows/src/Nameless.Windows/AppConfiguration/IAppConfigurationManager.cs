using System.Diagnostics.CodeAnalysis;

namespace Nameless.Windows.AppConfiguration;

/// <summary>
///     Defines the application configuration manager.
/// </summary>
public interface IAppConfigurationManager {
    /// <summary>
    ///     Tries to retrieve the configuration value from the configuration.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of the value.
    /// </typeparam>
    /// <param name="name">
    ///     The name of the configuration value.
    /// </param>
    /// <param name="output">
    ///     The configuration value, if it exists;
    ///     otherwise, <see langword="default"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/>, if the configuration value exists;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    bool TryGet<TValue>(string name, [NotNullWhen(returnValue: true)] out TValue? output);

    /// <summary>
    ///     Stores a value into the configuration.
    /// </summary>
    /// <typeparam name="TValue">
    ///     Type of the value to store.
    /// </typeparam>
    /// <param name="name">
    ///     The name of the configuration value.
    /// </param>
    /// <param name="value">
    ///     The value to store in the configuration.
    /// </param>
    void Set<TValue>(string name, TValue value);

    /// <summary>
    ///     Saves the changes done to the configuration.
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous action
    ///     execution.
    /// </returns>
    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Discards every in-memory change and reloads the configuration
    ///     from its backing file.
    /// </summary>
    /// <remarks>
    ///     Call this after the backing file has been replaced outside of
    ///     this manager (e.g. a restore that swapped <c>app.config</c> on
    ///     disk) so a later <see cref="SaveChangesAsync"/> — for instance
    ///     the one the host runs on application shutdown — writes back the
    ///     reloaded content instead of clobbering it with stale, pre-reload
    ///     in-memory values.
    /// </remarks>
    void Reload();
}