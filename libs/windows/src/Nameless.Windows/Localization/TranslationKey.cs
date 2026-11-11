using Nameless.Contracts;

namespace Nameless.Windows.Localization;

/// <summary>
///     The single place that defines how a section name maps to an anchor
///     <see cref="Type"/> and how a section and a key compose into the flat
///     lookup key <see cref="ITranslator"/> indexes with.
/// </summary>
public static class TranslationKey {
    /// <summary>
    ///     The separator used to compose a section and a key into a flat
    ///     lookup key.
    /// </summary>
    public const string Separator = "_";

    /// <summary>
    ///     Maps an anchor <see cref="Type"/> to its section name: its
    ///     <c>Name</c>, with generic arity stripped (e.g.
    ///     <c>Foo`1</c> becomes <c>Foo</c>).
    /// </summary>
    /// <param name="anchor">
    ///     The anchor type.
    /// </param>
    /// <returns>
    ///     The section name.
    /// </returns>
    public static string GetSection(Type anchor) {
        Throws.When.Null(anchor);

        var name = anchor.Name;
        var tick = name.IndexOf('`');

        return tick < 0 ? name : name[..tick];
    }

    /// <summary>
    ///     Composes <paramref name="section"/> and <paramref name="key"/>
    ///     into the flat lookup key <see cref="ITranslator"/> indexes with.
    /// </summary>
    /// <param name="section">
    ///     The section name.
    /// </param>
    /// <param name="key">
    ///     The section-relative key.
    /// </param>
    /// <returns>
    ///     The composed, flat lookup key.
    /// </returns>
    public static string Compose(string section, string key) {
        Throws.When.NullOrWhiteSpace(section);
        Throws.When.NullOrWhiteSpace(key);

        return $"{section}{Separator}{key}";
    }
}
