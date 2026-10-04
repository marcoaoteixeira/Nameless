using Microsoft.EntityFrameworkCore;

namespace Nameless.EntityFrameworkCore.Config;

/// <summary>
///     Entity type configurator (non-generic)
/// </summary>
public interface IEntityTypeConfigurator {
    /// <summary>
    ///     Applies the entity configuration.
    /// </summary>
    /// <param name="builder">
    ///     The model builder.
    /// </param>
    void Apply(ModelBuilder builder);
}