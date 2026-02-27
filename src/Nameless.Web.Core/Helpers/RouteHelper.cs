using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Nameless.Web.Helpers;

/// <summary>
///     Route helper class.
/// </summary>
public static class RouteHelper {
    /// <summary>
    ///     Retrieves the route parameters from a route pattern.
    /// </summary>
    /// <param name="routePattern">
    ///     The route pattern to extract parameters from.
    /// </param>
    /// <returns>
    ///     An enumerable collection of route parameter names.
    /// </returns>
    public static IEnumerable<string> GetRouteParameters([StringSyntax(Constants.Syntaxes.ROUTE)] string routePattern) {
        Throws.When.NullOrWhiteSpace(routePattern);

        var matches = Internals.RegexCache.RoutePattern().Matches(routePattern);

        foreach (var match in matches.OfType<Match>()) {
            // Strip constraints, e.g., {id:int} -> id
            yield return match.Groups[groupnum: 1].Value.Split(separator: ':')[0];
        }
    }
}