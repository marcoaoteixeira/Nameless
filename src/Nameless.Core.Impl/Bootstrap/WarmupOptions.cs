namespace Nameless.Bootstrap;

/// <summary>
///     The Bootstrap warmup options.
/// </summary>
public class WarmupOptions {
    /// <summary>
    ///     Gets or sets the context for <see cref="Bootstrapper"/>.
    /// </summary>
    public FlowContext Context { get; set; } = [];

    /// <summary>
    ///     Gets or sets the timeout in milliseconds for the warmup.
    /// </summary>
    public int Timeout { get; set; } = -1;
}