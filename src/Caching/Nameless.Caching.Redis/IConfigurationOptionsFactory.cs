using StackExchange.Redis;

namespace Nameless.Caching.Redis;

/// <summary>
/// Provides a contract for <see cref="ConfigurationOptions"/> creation.
/// </summary>
public interface IConfigurationOptionsFactory {
    /// <summary>
    /// Creates Redis configuration object.
    /// </summary>
    /// <returns>A <see cref="ConfigurationOptions"/> for Redis.</returns>
    ConfigurationOptions CreateConfigurationOptions();
}