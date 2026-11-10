namespace Nameless.Windows.Localization.Json;

/// <summary>
///     Event data for <see cref="Nameless.Windows.Localization.Json.ITranslationFileProvider.Changed"/>,
///     identifying which translation file key was modified on disk.
/// </summary>
/// <param name="key">
///     The culture name (e.g. <c>"pt-BR"</c>) or the literal
///     <c>"default"</c> whose backing file changed.
/// </param>
public sealed class TranslationFileChangedEventArgs(string key) : EventArgs {
    /// <summary>
    ///     The culture name (e.g. <c>"pt-BR"</c>) or the literal
    ///     <c>"default"</c> whose backing file changed.
    /// </summary>
    public string Key { get; } = key;
}
