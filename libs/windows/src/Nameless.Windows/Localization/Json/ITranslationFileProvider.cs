using System.Diagnostics.CodeAnalysis;

namespace Nameless.Windows.Localization.Json;

/// <summary>
///     Supplies the raw JSON content for a translation file, keyed by
///     culture name (e.g. <c>"pt-BR"</c>, <c>"pt"</c>) or the literal
///     <c>"default"</c> for the culture-less fallback file.
/// </summary>
/// <remarks>
///     This is the seam <see cref="Nameless.Windows.Localization.JsonTranslator"/>
///     looks up and caches through. <see cref="Changed"/> is the narrow
///     extension point a hot-reload-capable provider (backed by a
///     <see cref="System.IO.FileSystemWatcher"/>) uses to tell
///     <see cref="Nameless.Windows.Localization.JsonTranslator"/> which
///     culture to reload, without either side needing to know how the
///     other detects or represents the change on disk (#191).
/// </remarks>
public interface ITranslationFileProvider {
    /// <summary>
    ///     Raised when a provider that watches its backing storage (e.g.
    ///     a <see cref="System.IO.FileSystemWatcher"/>-backed
    ///     implementation) detects that a translation file changed on
    ///     disk. <see cref="Nameless.Windows.Localization.JsonTranslator"/>
    ///     subscribes to this to invalidate and reload just the affected
    ///     culture's cached entries and re-raise its own "Item[]"
    ///     <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
    ///     signal. Providers that do not watch for changes (e.g. a
    ///     read-once-at-startup implementation) never raise it, and do not
    ///     need to implement the event themselves — the default
    ///     implementation below is a no-op add/remove so adding this
    ///     event is not a breaking change for any existing implementer.
    /// </summary>
    event EventHandler<TranslationFileChangedEventArgs>? Changed {
        add { }
        remove { }
    }

    /// <summary>
    ///     Attempts to read the JSON content associated with
    ///     <paramref name="key"/>.
    /// </summary>
    /// <param name="key">
    ///     The culture name (e.g. <c>"pt-BR"</c>) or the literal
    ///     <c>"default"</c>.
    /// </param>
    /// <param name="content">
    ///     The raw JSON content, when a file for <paramref name="key"/>
    ///     exists; otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> when a file for <paramref name="key"/>
    ///     exists; otherwise <see langword="false"/>.
    /// </returns>
    bool TryRead(string key, [NotNullWhen(true)] out string? content);
}
