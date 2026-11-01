using Nameless.Configuration;

namespace Nameless.Windows.Bootstrap;

/// <summary>
///     The Bootstrap warmup options.
/// </summary>
[ConfigurationSectionName("Bootstrapper")]
public class BootstrapperOptions {
    /// <summary>
    ///     Gets or sets the timeout in milliseconds for the warmup.
    /// </summary>
    public int WarmupTimeout { get; set; } = -1;
}