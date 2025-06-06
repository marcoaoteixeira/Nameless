using NHibernate.Cfg;

namespace Nameless.NHibernate;

/// <summary>
/// Defines methods to create a configuration factory for NHibernate.
/// </summary>
public interface IConfigurationFactory {
    /// <summary>
    /// Creates and returns a new configuration instance.
    /// </summary>
    /// <returns>
    /// A <see cref="Configuration"/> object representing the newly created
    /// configuration.
    /// </returns>
    Configuration CreateConfiguration();
}