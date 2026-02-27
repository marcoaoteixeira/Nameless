namespace Nameless.Bootstrap.Infrastructure;

/// <summary>
///     Provides configuration options for registering and managing bootstrap
///     steps in an application startup sequence.
/// </summary>
public class BootstrapOptions {
    /// <summary>
    ///     Whether it should execute the steps in parallel or not.
    ///     Default is <see langword="true"/>.
    /// </summary>
    public bool EnableParallelExecution { get; set; } = true;

    /// <summary>
    ///     Gets or sets the maximum degree of parallelism.
    ///     Default value is <c>4</c>.
    /// </summary>
    /// <remarks>
    ///     Set the value to <c>-1</c> to define it as unlimited.
    /// </remarks>
    public int MaxDegreeOfParallelism { get; set; } = 4;
}