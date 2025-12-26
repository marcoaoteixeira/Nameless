using Microsoft.AspNetCore.OutputCaching;

namespace Nameless.Web.Infrastructure;

/// <summary>
///     Implements an output cache policy to ignore the client cache control.
/// </summary>
/// <remarks>
///     It is recommended to not use this policy in production environment.
/// </remarks>
public class IgnoreCacheControlPolicy : IOutputCachePolicy {
    /// <inheritdoc />
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation) {
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation) {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation) {
        return ValueTask.CompletedTask;
    }
}