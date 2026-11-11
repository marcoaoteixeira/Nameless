using System.Text.Json;
using Nameless.Windows.Localization;

namespace Nameless.WinApp.Resources.loc;

/// <summary>
///     Guards the JSON translation files (<c>default.json</c>,
///     <c>pt-BR.json</c> — ticket #189, regrouped by section in #197) from
///     drifting apart from each other now that the resx source of truth
///     they were originally compared against (<c>TranslationJsonParityTests</c>)
///     was retired with <c>ResourceTranslator</c> in #190. Missing keys
///     resolve silently to <c>"[key]"</c> at runtime (see
///     <c>JsonTranslator</c>), so a drifted key set would otherwise go
///     unnoticed until someone switches culture and spots the bracketed
///     placeholder.
/// </summary>
[IntegrationTest]
public class TranslationJsonKeyParityTests {
    // Mirrors JsonTranslator.ParseOptions — default JsonSerializerOptions
    // silently keep the last value for a duplicate property, which would
    // hide a duplicate key/section in these sample files from this guard.
    private static readonly JsonSerializerOptions ParseOptions = new() { AllowDuplicateProperties = false };

    [Fact]
    public void DefaultAndPtBrJson_ShareTheExactSameKeySet() {
        var defaultKeys = LoadJson("default.json");
        var ptBrKeys = LoadJson("pt-BR.json");

        Assert.True(defaultKeys.SetEquals(ptBrKeys),
            $"Key sets differ. Missing from pt-BR: [{string.Join(", ", defaultKeys.Except(ptBrKeys))}]. " +
            $"Missing from default: [{string.Join(", ", ptBrKeys.Except(defaultKeys))}].");
    }

    [Theory]
    [InlineData("default.json")]
    [InlineData("pt-BR.json")]
    public void TranslationJsonFile_NoKeyRedundantlyRepeatsItsOwnSectionPrefix(string fileName) {
        // #199: on-disk keys are section-relative — the section name is
        // written once (the JSON section header), not twice (also as a
        // "{Section}_" prefix on every key inside it, as #197 left them).
        var offenders = LoadSectionRelativeEntries(fileName)
            .Where(entry => entry.Key.StartsWith($"{entry.Section}_", StringComparison.Ordinal))
            .Select(entry => $"{entry.Section}/{entry.Key}")
            .ToArray();

        Assert.True(offenders.Length == 0,
            $"'{fileName}' has keys that redundantly repeat their own section as a prefix: [{string.Join(", ", offenders)}].");
    }

    /// <summary>
    ///     Loads the flat set of lookup keys out of a grouped translation
    ///     file (<c>{ "SectionName": { "Key1": "Text1" } }</c>),
    ///     flattening each entry to <see cref="TranslationKey.Compose(string, string)"/>
    ///     — the same composed key <c>JsonTranslator</c> builds at runtime
    ///     (#199; section-relative keys on disk).
    /// </summary>
    private static HashSet<string> LoadJson(string fileName) {
        var path = SysPath.Combine(AppContext.BaseDirectory, "Resources", "loc", fileName);
        var content = SysFile.ReadAllText(path);

        var sections = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(content, ParseOptions)
                       ?? throw new InvalidOperationException($"'{path}' deserialized to null.");

        return sections.SelectMany(section => section.Value.Keys
                                                      .Select(key => TranslationKey.Compose(section.Key, key)))
                       .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>
    ///     Loads every (section, section-relative key) pair out of a
    ///     grouped translation file, unflattened — used by the sanity
    ///     check above, which needs each key's own section name, not the
    ///     composed lookup key.
    /// </summary>
    private static IEnumerable<(string Section, string Key)> LoadSectionRelativeEntries(string fileName) {
        var path = SysPath.Combine(AppContext.BaseDirectory, "Resources", "loc", fileName);
        var content = SysFile.ReadAllText(path);

        var sections = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(content, ParseOptions)
                       ?? throw new InvalidOperationException($"'{path}' deserialized to null.");

        return sections.SelectMany(section => section.Value.Keys
                                                      .Select(key => (section.Key, key)));
    }
}
