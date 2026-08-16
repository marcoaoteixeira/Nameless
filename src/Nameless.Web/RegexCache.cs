using System.Text.RegularExpressions;

namespace Nameless.Web;

internal static partial class RegexCache {
    [GeneratedRegex(pattern: @"{([^}]+)}", RegexOptions.IgnoreCase)]
    internal static partial Regex RoutePattern();

    [GeneratedRegex(pattern: @"\{(\w+):[^}]+\}", RegexOptions.Compiled)]
    internal static partial Regex RouteParameterPattern();

    [GeneratedRegex(pattern: "[^a-zA-Z0-9]+", RegexOptions.Compiled)]
    internal static partial Regex OpenApiOperationIdSlugPattern();
}