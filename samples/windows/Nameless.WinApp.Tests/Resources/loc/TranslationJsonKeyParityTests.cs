using System.Text.Json;

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
    [Fact]
    public void DefaultAndPtBrJson_ShareTheExactSameKeySet() {
        var defaultKeys = LoadJson("default.json");
        var ptBrKeys = LoadJson("pt-BR.json");

        Assert.True(defaultKeys.SetEquals(ptBrKeys),
            $"Key sets differ. Missing from pt-BR: [{string.Join(", ", defaultKeys.Except(ptBrKeys))}]. " +
            $"Missing from default: [{string.Join(", ", ptBrKeys.Except(defaultKeys))}].");
    }

    /// <summary>
    ///     Loads the flat set of lookup keys out of a grouped translation
    ///     file (<c>{ "SectionName": [ { "Key1": "Text1" } ] }</c>) — the
    ///     section grouping is purely an on-disk filing convenience, so it
    ///     is flattened away here, same as <c>JsonTranslator</c> does at
    ///     runtime.
    /// </summary>
    private static HashSet<string> LoadJson(string fileName) {
        var path = SysPath.Combine(AppContext.BaseDirectory, "Resources", "loc", fileName);
        var content = SysFile.ReadAllText(path);

        var sections = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(content)
                       ?? throw new InvalidOperationException($"'{path}' deserialized to null.");

        return sections.Values
                       .SelectMany(entries => entries)
                       .SelectMany(entry => entry.Keys)
                       .ToHashSet(StringComparer.Ordinal);
    }
}
