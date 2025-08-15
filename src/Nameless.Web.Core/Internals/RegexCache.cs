using System.Text.RegularExpressions;

namespace Nameless.Web.Internals;

internal static partial class RegexCache {
    [GeneratedRegex(@"{([^}]+)}", RegexOptions.IgnoreCase)]
    internal static partial Regex RoutePattern();
}