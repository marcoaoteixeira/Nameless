#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Data.SQLite.Options;

public sealed record SQLiteOptions {
    public static SQLiteOptions Default => new();

    /// <summary>
    /// Gets or sets whether database will be set in-memory.
    /// Default is <c>true</c>.
    /// </summary>
    public bool UseInMemory { get; set; } = true;
        
    public string DatabasePath { get; set; } = $".{Path.DirectorySeparatorChar}database.db";

    public string? Password { get; set; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(Password))]
#endif
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Password);

    public string GetConnectionString() {
        var connStr = string.Empty;

        connStr += $"Data Source={(UseInMemory ? ":memory:" : DatabasePath)};";
        connStr += UseCredentials ? $"Password={Password};" : string.Empty;

        return connStr;
    }
}