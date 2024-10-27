using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

internal sealed record CacheEntry {
    internal static CacheEntry Empty => new(Translation.Empty, NullDisposable.Instance);

    internal Translation Translation { get; }

    internal IDisposable FileChangeCallback { get; }

    internal CacheEntry(Translation translation, IDisposable fileChangeCallback) {
        Translation = translation;
        FileChangeCallback = fileChangeCallback;
    }
}
