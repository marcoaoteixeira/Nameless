using Nameless.Attributes;

namespace Nameless.Testing.Tools.Containers.Postgres;

/// <summary>
///     Represents configuration options for a PostgreSql test container instance.
/// </summary>
[ConfigurationSectionName("PostgresServer")]
public class PostgresServerOptions : TestcontainersOptions<PostgresServerOptions>
{
    /// <summary>
    ///     Whether it should enable security.
    /// </summary>
    public bool EnableSecurity => !string.IsNullOrWhiteSpace(Username) &&
                                  !string.IsNullOrWhiteSpace(Password);

    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the database name.
    /// </summary>
    public string Database { get; set; } = "postgres";

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PostgresServerOptions"/> class.
    /// </summary>
    public PostgresServerOptions()
    {
        ContainerPort = 5432;
        HostPort = 5432;
    }
}
