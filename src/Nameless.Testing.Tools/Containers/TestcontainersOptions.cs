using System.Net;

namespace Nameless.Testing.Tools.Containers;

/// <summary>
///     Provides a base class for configuring options related to test containers,
///     including registry, image, port, and environment settings.
/// </summary>
public abstract class TestcontainersOptions<TSelf>
    where TSelf : TestcontainersOptions<TSelf>
{
    /// <summary>
    ///     Gets or sets the registry URL.
    /// </summary>
    public string? RegistryUrl { get; set; }

    /// <summary>
    ///     Gets or sets the image URL.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the container port.
    /// </summary>
    public int ContainerPort { get; set; }

    /// <summary>
    ///     Gets or sets the host port.
    /// </summary>
    public int HostPort { get; set; }

    /// <summary>
    ///     Gets or sets a collection of environment
    ///     variables.
    /// </summary>
    public Dictionary<string, string> Environment { get; set; } = [];

    /// <summary>
    ///     Validates the properties of the current instance to ensure that
    ///     required values are set and within acceptable ranges.
    /// </summary>
    /// <remarks>
    ///     This method checks that the <see cref="Image"/> property is not
    ///     <see langword="null"/> or whitespace, and that
    ///     <see cref="HostPort"/> and <see cref="ContainerPort"/> are within
    ///     the valid range of port numbers.
    ///     If any validation fails, an exception is thrown with a descriptive
    ///     message.
    /// </remarks>
    /// <returns>
    ///     The current instance of the class.
    /// </returns>
    public virtual TSelf Validate()
    {
        Throws.When.NullOrWhiteSpace(
            Image,
            message: $"Configuration '{nameof(Image)}' property value must be provided."
        );

        Throws.When.OutOfRange(
            HostPort,
            minimumValue: IPEndPoint.MinPort,
            maximumValue: IPEndPoint.MaxPort,
            message: $"Invalid value for configuration '{nameof(HostPort)}' property: {HostPort}"
        );

        Throws.When.OutOfRange(
            ContainerPort,
            minimumValue: IPEndPoint.MinPort,
            maximumValue: IPEndPoint.MaxPort,
            message: $"Invalid value for configuration '{nameof(ContainerPort)}' property: {ContainerPort}"
        );

        return (TSelf)this;
    }
}
