using System.Diagnostics.CodeAnalysis;

namespace Nameless.Windows.Localization.Json;

/// <summary>
///     Supplies the raw JSON content for a translation file, keyed by
///     culture name (e.g. <c>"pt-BR"</c>, <c>"pt"</c>) or the literal
///     <c>"default"</c> for the culture-less fallback file.
/// </summary>
/// <remarks>
///     This is the seam <see cref="Nameless.Windows.Localization.JsonTranslator"/>
///     looks up and caches through. A hot-reload-capable provider (backed by a
///     <see cref="System.IO.FileSystemWatcher"/>) can be swapped in later
///     without reworking <see cref="Nameless.Windows.Localization.JsonTranslator"/>'s
///     lookup/cache logic.
/// </remarks>
public interface ITranslationFileProvider {
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
