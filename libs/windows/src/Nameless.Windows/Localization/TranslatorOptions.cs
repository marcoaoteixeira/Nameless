using Nameless.Configuration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Configuration options for the translator subsystem. Bound from the
///     <c>Translator</c> configuration section.
/// </summary>
[ConfigurationSectionName("Translator")]
public record TranslatorOptions {
    /// <summary>
    ///     Root path, relative to the application's base directory, where
    ///     the per-culture JSON translation files
    ///     (<c>{culture}.json</c>, <c>default.json</c>) are located.
    ///     Used by JSON-backed <see cref="Json.ITranslationFileProvider"/>
    ///     implementations.
    /// </summary>
    public string? RootPath { get; init; }

    /// <summary>
    ///     BCP 47 culture tag (e.g. <c>"pt-BR"</c>) intended to seed the
    ///     starting <see cref="ITranslator.CurrentCulture"/> before any
    ///     explicit <see cref="ITranslator.SetCulture(string)"/> call.
    /// </summary>
    /// <remarks>
    ///     Not yet consumed by any shipped <see cref="ITranslator"/>
    ///     implementation — <see cref="JsonTranslator"/> always starts from
    ///     <see cref="System.Globalization.CultureInfo.CurrentUICulture"/>
    ///     regardless of this value. Tracked as a follow-up; do not rely on
    ///     this property having any effect yet.
    /// </remarks>
    public string? DefaultCulture { get; init; }

    /// <summary>
    ///     How long a <see cref="System.IO.FileSystemWatcher"/>-backed
    ///     <see cref="Json.ITranslationFileProvider"/> waits after the last
    ///     file-system event for a given file before reloading it and
    ///     raising <see cref="Json.ITranslationFileProvider.Changed"/>.
    ///     Coalesces the burst of events a single save can raise (and
    ///     successive quick edits) into a single reload. Defaults to 300
    ///     milliseconds.
    /// </summary>
    public TimeSpan WatcherDebounceDelay { get; init; } = TimeSpan.FromMilliseconds(300);
}