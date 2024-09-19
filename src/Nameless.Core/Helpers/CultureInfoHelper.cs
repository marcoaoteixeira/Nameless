using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Helpers;

/// <summary>
/// <see cref="CultureInfo"/> helpers.
/// </summary>
public static class CultureInfoHelper {
    /// <summary>
    /// Tries to create a <see cref="CultureInfo"/> instance by its name.
    /// </summary>
    /// <param name="cultureName">The culture name</param>
    /// <param name="culture">The output</param>
    /// <returns><c>true</c> if created; otherwise <c>false</c>.</returns>
    public static bool TryCreateCultureInfo(string cultureName, [NotNullWhen(returnValue: true)] out CultureInfo? culture) {
        culture = null;

        var result = false;
        try {
            culture = CultureInfo.GetCultureInfo(cultureName);
            result = true;
        } catch {
            /* ignore */
        }

        return result;
    }
}