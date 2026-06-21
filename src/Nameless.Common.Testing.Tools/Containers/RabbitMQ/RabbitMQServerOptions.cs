using Nameless.Attributes;

namespace Nameless.Testing.Tools.Containers.RabbitMQ;

/// <summary>
///     Represents configuration options for a PostgreSql test container instance.
/// </summary>
[ConfigurationSectionName("RabbitMQServer")]
public class RabbitMQServerOptions : TestcontainersOptions<RabbitMQServerOptions>
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
    ///     Gets the management portal container port
    /// </summary>
    public int ManagementContainerPort { get; set; }

    /// <summary>
    ///     Gets the management portal host port
    /// </summary>
    public int ManagementHostPort { get; set; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="RabbitMQServerOptions"/> class.
    /// </summary>
    public RabbitMQServerOptions()
    {
        ContainerPort = 5672;
        HostPort = 5672;

        ManagementContainerPort = 15672;
        ManagementHostPort = 15672;
    }
}
