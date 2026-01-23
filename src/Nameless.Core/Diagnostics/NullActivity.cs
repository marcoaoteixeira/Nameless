using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     A Null-Object pattern implementation of <see cref="IActivity"/>.
/// </summary>
public sealed class NullActivity : IActivity {
    /// <summary>
    ///     Gets the unique instance of <see cref="NullActivity"/>.
    /// </summary>
    public static IActivity Instance { get; } = new NullActivity();

    static NullActivity() { }

    private NullActivity() { }

    /// <inheritdoc />
    public IActivity SetTag(string key, object? value) {
        return this;
    }

    /// <inheritdoc />
    public IActivity AddException(Exception exception) {
        return this;
    }

    /// <inheritdoc />
    public IActivity SetStatus(ActivityStatusCode code, string? description) {
        return this;
    }

    /// <inheritdoc />
    public void Dispose() { }
}