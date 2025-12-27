using System.Text.RegularExpressions;

namespace Nameless.Lucene;

internal static partial class RegexCache {
    [GeneratedRegex(pattern: @"^[A-Za-z0-9_\-\s]+$", RegexOptions.IgnoreCase)]
    internal static partial Regex IndexNamePattern();
}