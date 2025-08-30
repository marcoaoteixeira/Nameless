using System.Diagnostics.CodeAnalysis;

namespace Nameless.Data.Sqlite;

/// <summary>
/// Sqlite options.
/// </summary>
public sealed class SqliteOptions {
    /// <summary>
    ///     Whether database will be set as "in-memory".
    /// </summary>
    public bool UseInMemory { get; set; }

    /// <summary>
    /// Gets or sets the database path.
    /// </summary>
    public string DatabasePath { get; set; } = Path.Combine(".", "database.db");

    /// <summary>
    /// Gets or sets the database password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Whether it will use credentials to access the database.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Password))]
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    /// Retrieves the connection string given the parameters of the option.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> represeting the connection string.
    /// </returns>
    public string GetConnectionString() {
        var connStr = string.Empty;

        connStr += $"Data Source={(UseInMemory ? ":memory:" : DatabasePath)};";
        connStr += UseCredentials && !UseInMemory
            ? $"Password={Password};"
            : string.Empty;

        return connStr;
    }
}