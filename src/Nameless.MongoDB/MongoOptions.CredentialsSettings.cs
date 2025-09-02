using System.Diagnostics.CodeAnalysis;

namespace Nameless.MongoDB;

/// <summary>
/// Represents the settings for MongoDB credentials.
/// </summary>
public sealed record CredentialsSettings {
    /// <summary>
    /// Whether to use credentials for MongoDB connection.
    /// </summary>

    [MemberNotNullWhen(returnValue: true, nameof(Database), nameof(Mechanism), nameof(Username), nameof(Password))]
    internal bool UseCredentials => !string.IsNullOrWhiteSpace(Database) &&
                                    !string.IsNullOrWhiteSpace(Mechanism) &&
                                    !string.IsNullOrWhiteSpace(Username) &&
                                    !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    /// Gets or sets the name of the database to authenticate against.
    /// Default is "admin".
    /// </summary>
    public string Database { get; set; } = "admin";

    /// <summary>
    /// Gets or sets the authentication mechanism to use.
    /// Default is "SCRAM-SHA-256".
    /// </summary>
    public string Mechanism { get; set; } = "SCRAM-SHA-256";

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string? Password { get; set; }
}