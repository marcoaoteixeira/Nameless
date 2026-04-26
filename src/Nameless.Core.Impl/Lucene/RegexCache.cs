using System.Text.RegularExpressions;

namespace Nameless.Lucene;

public static partial class RegexCache {
    [GeneratedRegex(pattern: @"^[A-Za-z0-9_\-\s=]+$", RegexOptions.IgnoreCase)]
    public static partial Regex IndexNamePattern();
}