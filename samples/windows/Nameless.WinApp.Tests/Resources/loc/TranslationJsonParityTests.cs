using System.Globalization;
using System.Resources;
using System.Text.Json;

namespace Nameless.WinApp.Resources.loc;

/// <summary>
///     Pins the new flat JSON translation files (<c>default.json</c>,
///     <c>pt-BR.json</c> — ticket #189) against the still-active, embedded
///     resx resources (<c>TranslationResources.resx</c>,
///     <c>TranslationResources.pt-BR.resx</c>) they were converted from:
///     every key/value pair must match exactly, in both directions, for
///     both cultures. <c>ResourceTranslator</c> remains wired until #190
///     swaps it for <c>JsonTranslator</c> — this guards against the JSON
///     files silently drifting from the resx source of truth until then.
/// </summary>
[IntegrationTest]
public class TranslationJsonParityTests {
    private const string ResourceBaseName = "Nameless.WinApp.Resources.loc.TranslationResources";

    [Fact]
    public void DefaultJson_MatchesTheCultureLessResxResourceSet_KeyForKeyAndValueForValue() {
        AssertJsonMatchesResourceSet(jsonFileName: "default.json", culture: CultureInfo.InvariantCulture);
    }

    [Fact]
    public void PtBrJson_MatchesThePtBrResxResourceSet_KeyForKeyAndValueForValue() {
        AssertJsonMatchesResourceSet(jsonFileName: "pt-BR.json", culture: CultureInfo.GetCultureInfo("pt-BR"));
    }

    [Fact]
    public void DefaultAndPtBrJson_ShareTheExactSameKeySet() {
        var defaultKeys = LoadJson("default.json").Keys.ToHashSet(StringComparer.Ordinal);
        var ptBrKeys = LoadJson("pt-BR.json").Keys.ToHashSet(StringComparer.Ordinal);

        Assert.True(defaultKeys.SetEquals(ptBrKeys),
            $"Key sets differ. Missing from pt-BR: [{string.Join(", ", defaultKeys.Except(ptBrKeys))}]. " +
            $"Missing from default: [{string.Join(", ", ptBrKeys.Except(defaultKeys))}].");
    }

    private static void AssertJsonMatchesResourceSet(string jsonFileName, CultureInfo culture) {
        var json = LoadJson(jsonFileName);
        var resx = LoadResourceSet(culture);

        var missingFromJson = resx.Keys.Except(json.Keys, StringComparer.Ordinal).ToArray();
        var extraInJson = json.Keys.Except(resx.Keys, StringComparer.Ordinal).ToArray();

        Assert.NotEmpty(resx);
        Assert.Empty(missingFromJson);
        Assert.Empty(extraInJson);

        foreach (var (key, resxValue) in resx) {
            Assert.Equal(resxValue, json[key]);
        }
    }

    private static Dictionary<string, string> LoadJson(string fileName) {
        var path = SysPath.Combine(AppContext.BaseDirectory, "Resources", "loc", fileName);
        var content = SysFile.ReadAllText(path);

        return JsonSerializer.Deserialize<Dictionary<string, string>>(content)
               ?? throw new InvalidOperationException($"'{path}' deserialized to null.");
    }

    private static Dictionary<string, string> LoadResourceSet(CultureInfo culture) {
        var resourceManager = new ResourceManager(ResourceBaseName, typeof(App).Assembly);
        var resourceSet = resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false)
                           ?? throw new InvalidOperationException($"No resource set for culture '{culture.Name}'.");

        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in resourceSet.Cast<System.Collections.DictionaryEntry>()) {
            result[(string)entry.Key] = (string)entry.Value!;
        }

        return result;
    }
}
