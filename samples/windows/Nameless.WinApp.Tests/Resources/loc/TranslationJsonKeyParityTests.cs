using System.Text.Json;

namespace Nameless.WinApp.Resources.loc;

/// <summary>
///     Guards the flat JSON translation files (<c>default.json</c>,
///     <c>pt-BR.json</c> — ticket #189) from drifting apart from each
///     other now that the resx source of truth they were originally
///     compared against (<c>TranslationJsonParityTests</c>) was retired
///     with <c>ResourceTranslator</c> in #190. Missing keys resolve
///     silently to <c>"[key]"</c> at runtime (see <c>JsonTranslator</c>),
///     so a drifted key set would otherwise go unnoticed until someone
///     switches culture and spots the bracketed placeholder.
/// </summary>
[IntegrationTest]
public class TranslationJsonKeyParityTests {
    [Fact]
    public void DefaultAndPtBrJson_ShareTheExactSameKeySet() {
        var defaultKeys = LoadJson("default.json").Keys.ToHashSet(StringComparer.Ordinal);
        var ptBrKeys = LoadJson("pt-BR.json").Keys.ToHashSet(StringComparer.Ordinal);

        Assert.True(defaultKeys.SetEquals(ptBrKeys),
            $"Key sets differ. Missing from pt-BR: [{string.Join(", ", defaultKeys.Except(ptBrKeys))}]. " +
            $"Missing from default: [{string.Join(", ", ptBrKeys.Except(defaultKeys))}].");
    }

    private static Dictionary<string, string> LoadJson(string fileName) {
        var path = SysPath.Combine(AppContext.BaseDirectory, "Resources", "loc", fileName);
        var content = SysFile.ReadAllText(path);

        return JsonSerializer.Deserialize<Dictionary<string, string>>(content)
               ?? throw new InvalidOperationException($"'{path}' deserialized to null.");
    }
}
