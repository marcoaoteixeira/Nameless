using System.Diagnostics.CodeAnalysis;

namespace Nameless.Diagnostics;

/// <summary>
///     Provides configuration options for customizing the activity source
///     used in telemetry or tracing scenarios.
/// </summary>
public class ActivitySourceOptions {
    /// <summary>
    ///     Gets or sets the unique name used to identify the activity source.
    /// </summary>
    public string? UniqueActivitySourceName { get; set; }

    /// <summary>
    ///     Gets or sets the unique version identifier for the activity source.
    /// </summary>
    public string? UniqueActivitySourceVersion { get; set; }

    /// <summary>
    ///     Whether it should create a unique activity source given the name
    ///     specified.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(UniqueActivitySourceName))]
    public bool UseUniqueActivitySource
        => !string.IsNullOrWhiteSpace(UniqueActivitySourceName);
}