using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Nameless.Helpers;

namespace Nameless.IO.System;

/// <summary>
///     Factory of <see cref="FileProvider" />
/// </summary>
public class FileProviderFactory {
    private readonly ConcurrentDictionary<string, FileProvider> _cache = [];

    /// <summary>
    ///     Gets or creates a new <see cref="FileProvider"/>.
    /// </summary>
    /// <param name="configure">
    ///     The configuration delegate.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="FileProvider"/>.
    /// </returns>
    /// <remarks>
    ///     It caches the instance of <see cref="FileProvider"/> by the
    ///     <see cref="FileProviderOptions.Root"/> value.
    /// </remarks>
    public FileProvider GetOrCreate(Action<FileProviderOptions> configure) {
        var opts = ActionHelper.FromDelegate(configure);
        var key = PathHelper.Normalize(opts.Root).ToLowerInvariant();

        return _cache.GetOrAdd(key, _ => new FileProvider(
            Options.Create(opts)
        ));
    }
}
