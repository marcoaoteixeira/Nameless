using System.Diagnostics.CodeAnalysis;

namespace Nameless.Data.Sqlite;

/// <summary>
/// Sqlite options.
/// </summary>
public class SqliteOptions {
    /// <summary>
    /// Whether it will use credentials to access the database.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Password))]
    internal bool UseCredentials
        => !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    ///     Whether database will be set as "in-memory".
    /// </summary>
    public bool UseInMemory { get; set; }

    /// <summary>
    /// Gets or sets the database path.
    /// </summary>
    public string DatabaseFilePath { get; set; } = Path.Combine(path1: ".", path2: "database.db");

    /// <summary>
    /// Gets or sets the database password.
    /// </summary>
    public string? Password { get; set; }
}