using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
/// Default implementation of <see cref="IPluralizationRuleProvider"/>.
/// </summary>
public sealed class PluralizationRuleProvider : IPluralizationRuleProvider {
    // ReSharper disable once InconsistentNaming
    private readonly ConcurrentDictionary<string, PluralizationRuleDelegate> Cache = [];

    public readonly PluralizationRuleDelegate DefaultRule = count => count >= 1 ? 1 : 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluralizationRuleProvider"/> class.
    /// </summary>
    public PluralizationRuleProvider() {
        // ReSharper disable ConvertConditionalTernaryExpressionToSwitchExpression
        AddRule(
        [
            "ay", "bo", "cgg", "dz", "fa", "id", "ja", "jbo", "ka", "kk", "km", "ko", "ky", "lo", "ms", "my", "sah",
            "su", "th", "tt", "ug", "vi", "wo", "zh"
        ], _ => 0);
        AddRule(
        [
            "ach", "ak", "am", "arn", "br", "fil", "fr", "gun", "ln", "mfe", "mg", "mi", "oc", "pt-BR", "tg", "ti",
            "tr", "uz", "wa"
        ], n => n > 1 ? 1 : 0);
        AddRule(
        [
            "af", "an", "anp", "as", "ast", "az", "bg", "bn", "brx", "ca", "da", "de", "doi", "el", "en", "eo", "es",
            "es-AR", "et", "eu", "ff", "fi", "fo", "fur", "fy", "gl", "gu", "ha", "he", "hi", "hne", "hu", "hy", "ia",
            "it", "kl", "kn", "ku", "lb", "mai", "ml", "mn", "mni", "mr", "nah", "nap", "nb", "ne", "nl", "nn", "no",
            "nso", "or", "pa", "pap", "pms", "ps", "pt", "rm", "rw", "sat", "sco", "sd", "se", "si", "so", "son", "sq",
            "sv", "sw", "ta", "te", "tk", "ur", "yo"
        ], n => n != 1 ? 1 : 0);
        AddRule(["is"], n => n % 10 != 1 || n % 100 == 11 ? 1 : 0);
        AddRule(["jv"], n => n != 0 ? 1 : 0);
        AddRule(["mk"], n => n == 1 || n % 10 == 1 ? 0 : 1);
        AddRule(["be", "bs", "hr", "lt"],
            n => n % 10 == 1 && n % 100 != 11 ? 0 :
                n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2);
        AddRule(["cs"], n => n == 1 ? 0 : n is >= 2 and <= 4 ? 1 : 2);
        AddRule(["csb", "pl"], n => n == 1 ? 0 : n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2);
        AddRule(["lv"], n => n % 10 == 1 && n % 100 != 11 ? 0 : n != 0 ? 1 : 2);
        AddRule(["mnk"], n => n == 0 ? 0 : n == 1 ? 1 : 2);
        AddRule(["ro"], n => n == 1 ? 0 : n == 0 || (n % 100 > 0 && n % 100 < 20) ? 1 : 2);
        AddRule(["cy"], n => n == 1 ? 0 : n == 2 ? 1 : n is not 8 and not 11 ? 2 : 3);
        AddRule(["gd"], n => n is 1 or 11 ? 0 : n is 2 or 12 ? 1 : n is > 2 and < 20 ? 2 : 3);
        AddRule(["kw"], n => n == 1 ? 0 : n == 2 ? 1 : n == 3 ? 2 : 3);
        AddRule(["mt"],
            n => n == 1 ? 0 : n == 0 || (n % 100 > 1 && n % 100 < 11) ? 1 : n % 100 is > 10 and < 20 ? 2 : 3);
        AddRule(["sl"], n => n % 100 == 1 ? 1 : n % 100 == 2 ? 2 : n % 100 is 3 or 4 ? 3 : 0);
        AddRule(["ru", "sr", "uk"],
            n => n % 10 == 1 && n % 100 != 11 ? 0 :
                n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20) ? 1 : 2);
        AddRule(["sk"], n => n == 1 ? 0 : n is >= 2 and <= 4 ? 1 : 2);
        AddRule(["ga"], n => n == 1 ? 0 : n == 2 ? 1 : n is > 2 and < 7 ? 2 : n is > 6 and < 11 ? 3 : 4);
        AddRule(["ar"],
            n => n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 is >= 3 and <= 10 ? 3 : n % 100 >= 11 ? 4 : 5);
        // ReSharper restore ConvertConditionalTernaryExpressionToSwitchExpression
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="culture"/> is <see langword="null"/>.
    /// </exception>
    public bool TryGet(CultureInfo culture, [NotNullWhen(returnValue: true)] out PluralizationRuleDelegate? rule) {
        rule = null;

        if (string.IsNullOrWhiteSpace(culture.Name)) {
            return false;
        }

        return Cache.TryGetValue(culture.Name, out rule)
               || TryGet(culture.Parent, out rule);
    }

    private void AddRule(IEnumerable<string> cultures, PluralizationRuleDelegate rule) {
        foreach (var culture in cultures) {
            Cache[culture] = rule;
        }
    }
}