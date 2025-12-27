using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
///     Provides methods to retrieve pluralization rules.
/// </summary>
public interface IPluralizationRuleProvider {
    /// <summary>
    ///     Tries to retrieve a valid pluralization rule delegate for the culture.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <param name="rule">The output pluralization rule delegate.</param>
    /// <returns><see langword="true"/> if pluralization rule was found; otherwise <see langword="false"/>.</returns>
    bool TryGet(CultureInfo culture, [NotNullWhen(returnValue: true)] out PluralizationRuleDelegate? rule);
}