using Nameless.Attributes;

namespace Nameless.Web.OutputCache;

/// <summary>
///     Provides configuration options for output cache policies,
///     including predefined cache durations.
/// </summary>
[ConfigurationSectionName("OutputCachePolicy")]
public class OutputCachePolicyOptions
{
    /// <summary>
    ///     Gets or sets the collection of output cache policies.
    /// </summary>
    /// <remarks>
    ///     If not set, the property provides a default set of standard
    ///     cache policies, including five seconds and one minute, which
    ///     can be used to configure output cache consistently across
    ///     the application.
    /// </remarks>
    public OutputCachePolicy[] Entries { get; set; } = [
        OutputCachePolicy.OneSecond,
        OutputCachePolicy.OneMinute,
        OutputCachePolicy.FifteenSeconds,
        OutputCachePolicy.ThirtySeconds,
        OutputCachePolicy.OneMinute
    ];
}