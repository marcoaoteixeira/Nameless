using System.Text.RegularExpressions;

namespace Nameless.Lucene;

/// <summary>
///     Provides source-generated, cached <see cref="Regex"/> instances used across the Lucene library.
/// </summary>
public static partial class RegexCache {
    /// <summary>
    ///     Returns a compiled <see cref="Regex"/> that matches valid Lucene index names,
    ///     allowing alphanumeric characters, underscores, hyphens, whitespace, and equals signs.
    /// </summary>
    /// <returns>The compiled <see cref="Regex"/> instance.</returns>
    [GeneratedRegex(pattern: @"^[A-Za-z0-9_\-\s=]+$", RegexOptions.IgnoreCase)]
    public static partial Regex IndexNamePattern();
}