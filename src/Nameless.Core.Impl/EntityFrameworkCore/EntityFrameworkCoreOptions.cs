using Nameless.Attributes;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Configuration options for the Entity Framework Core integration.
/// </summary>
[ConfigurationSectionName("EntityFrameworkCore")]
public record EntityFrameworkCoreOptions {
    /// <summary>
    ///     Gets or sets the name of the connection string to use when
    ///     configuring the <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
    ///     Defaults to <c>"default"</c>.
    /// </summary>
    public string ConnectionStringName { get; init; } = "default";
}