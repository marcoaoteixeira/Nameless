using System.Text.RegularExpressions;

namespace Nameless;

public static partial class RegexCache {
    [GeneratedRegex(@"^v?((?:\d+\.){2}\d+)(?:-[0-9A-Za-z.-]+)?(?:\+[0-9A-Za-z.-]+)?$", RegexOptions.IgnoreCase)]
    public static partial Regex SemanticVersionPattern();
}
