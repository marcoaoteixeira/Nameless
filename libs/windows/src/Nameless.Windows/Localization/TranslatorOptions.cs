using Nameless.Configuration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Configuration options for the translator subsystem. Bound from the
///     <c>Translator</c> configuration section.
/// </summary>
[ConfigurationSectionName("Translator")]
public record TranslatorOptions {
    /// <summary>
    ///     Assembly-qualified name of the generated resource class
    ///     (e.g. <c>"MyApp.Resources.Translations, MyApp"</c>).
    ///     Used to locate the <see cref="System.Resources.ResourceManager"/>
    ///     at runtime.
    /// </summary>
    public string? ResourceFullName { get; init; }

    /// <summary>
    ///     Root path, relative to the application's base directory, where
    ///     the per-culture JSON translation files
    ///     (<c>{culture}.json</c>, <c>default.json</c>) are located.
    ///     Used by JSON-backed <see cref="Json.ITranslationFileProvider"/>
    ///     implementations.
    /// </summary>
    public string? RootPath { get; init; }
}