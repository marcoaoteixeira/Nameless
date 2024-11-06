namespace Nameless;

/// <summary>
/// Singleton Pattern implementation for <see cref="IProgress{T}" />.
/// See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class NullProgress<T> : IProgress<T> {
    /// <summary>
    /// Gets the unique instance of <see cref="NullProgress{T}" />.
    /// </summary>
    public static IProgress<T> Instance { get; } = new NullProgress<T>();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullProgress() { }

    private NullProgress() { }

    /// <inheritdoc/>
    public void Report(T value) { }
}

/// <summary>
/// Singleton Pattern implementation for <see cref="IProgress{T}" /> where T is <see cref="int"/>.
/// See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class NullProgress : IProgress<int> {
    /// <summary>
    /// Gets the unique instance of <see cref="NullProgress" />.
    /// </summary>
    public static IProgress<int> Instance { get; } = new NullProgress();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullProgress() { }

    private NullProgress() { }

    /// <inheritdoc/>
    public void Report(int value) { }
}