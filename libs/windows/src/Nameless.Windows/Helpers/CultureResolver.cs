using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Windows.Helpers;

/// <summary>
///     Culture Resolver
/// </summary>
public static class CultureResolver {
    /// <summary>
    ///     Tries retrieve <see cref="CultureInfo"/> for the specified
    ///     culture tag.
    /// </summary>
    /// <param name="tag">
    ///     The culture tag.
    /// </param>
    /// <param name="output">
    ///     The culture info.
    /// </param>
    /// <returns>
    ///     <see cref="CultureInfo"/>
    /// </returns>
    public static bool TryGet(string tag, [NotNullWhen(returnValue: true)] out CultureInfo? output) {
        output = null;

        if (string.IsNullOrWhiteSpace(tag)) { return false; }

        try {
            // Normalize "pt_BR" -> "pt-BR"
            var culture = CultureInfo.GetCultureInfo(
                tag.Trim().Replace(StaticData.Separators.Underscore, StaticData.Separators.Dash)
            );

            // With ICU, unknown tags can be used as "makeup culture"
            output = culture.LCID == 0x1000 /* LOCALE_CUSTOM_UNSPECIFIED */ ? null : culture;
        }
        catch (CultureNotFoundException) { return false; }

        return output is not null;
    }
}
