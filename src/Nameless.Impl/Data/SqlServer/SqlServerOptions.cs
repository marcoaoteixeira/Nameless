using System.Diagnostics.CodeAnalysis;
using Nameless.Attributes;

namespace Nameless.Data.SqlServer;

/// <summary>
/// MS SQL Server options.
/// </summary>
[ConfigurationSectionName("SqlServer")]
public record SqlServerOptions {
    /// <summary>
    /// Whether it will use credentials to access the database.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password) &&
           !UseIntegratedSecurity;

    /// <summary>
    /// Gets or sets the server address.
    /// </summary>
    public string Server { get; init; } = "(localdb)\\MSSQLLocalDB";

    /// <summary>
    /// Gets or sets the database to use.
    /// </summary>
    public string Database { get; init; } = "master";

    /// <summary>
    /// Gets or sets the credentials username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Gets or sets the credentials password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the application should use an attached database.
    /// </summary>
    public bool UseAttachedDb { get; init; }

    /// <summary>
    /// Whether it should use integrated security.
    /// </summary>
    public bool UseIntegratedSecurity { get; init; }

    /// <summary>
    ///     Gets or sets the connection string name.
    /// </summary>
    /// <remarks>
    ///     If provided, it will be the preferred connection string over
    ///     the current configuration.
    /// </remarks>
    public string? ConnectionStringName { get; init; }
}