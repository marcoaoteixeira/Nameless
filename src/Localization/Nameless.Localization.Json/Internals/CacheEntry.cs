using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

internal sealed record CacheEntry {
    internal static CacheEntry Empty => new(Resource.Empty, NullDisposable.Instance);

    internal Resource Resource { get; }

    internal IDisposable FileChangeCallback { get; }

    internal CacheEntry(Resource resource, IDisposable fileChangeCallback) {
        Resource = resource;
        FileChangeCallback = fileChangeCallback;
    }
}
