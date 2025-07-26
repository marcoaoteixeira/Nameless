using System.Text.RegularExpressions;

namespace Nameless.Web;

internal static partial class RegexCache {
    [GeneratedRegex(@"{([^}]+)}", RegexOptions.IgnoreCase)]
    internal static partial Regex RoutePattern();
}