namespace Nameless;

/// <summary>
/// Singleton Pattern implementation for <see cref="IDisposable" />.
/// See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class NullDisposable : IDisposable {
    /// <summary>
    /// Gets the unique instance of <see cref="NullDisposable" />.
    /// </summary>
    public static IDisposable Instance { get; } = new NullDisposable();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullDisposable() { }

    private NullDisposable() { }

    public void Dispose() { }
}