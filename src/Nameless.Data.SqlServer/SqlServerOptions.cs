﻿using System.Diagnostics.CodeAnalysis;

namespace Nameless.Data.SqlServer;

/// <summary>
/// MS SQL Server options.
/// </summary>
public sealed record SqlServerOptions {
    /// <summary>
    /// Gets or sets the server address.
    /// </summary>
    public string Server { get; set; } = "(localdb)\\MSSQLLocalDB";

    /// <summary>
    /// Gets or sets the database to use.
    /// </summary>
    public string Database { get; set; } = "master";

    /// <summary>
    /// Gets or sets the credentials username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the credentials password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application should use an attached database.
    /// </summary>
    public bool UseAttachedDb { get; set; }

    /// <summary>
    /// Whether it should use integrated security.
    /// </summary>
    public bool UseIntegratedSecurity { get; set; }

    /// <summary>
    /// Whether it will use credentials to access the database.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Username), nameof(Password))]
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password) &&
           !UseIntegratedSecurity;

    /// <summary>
    /// Retrieves the connection string given the parameters of the option.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> represeting the connection string.
    /// </returns>
    public string GetConnectionString() {
        var connStr = string.Empty;

        connStr += $"Server={Server};";

        connStr += UseAttachedDb
            ? $"AttachDbFilename={Database};"
            : $"Database={Database};";

        connStr += UseCredentials
            ? $"User Id={Username};Password={Password};"
            : string.Empty;

        connStr += UseIntegratedSecurity
            ? "Integrated Security=true;"
            : string.Empty;

        return connStr;
    }
}