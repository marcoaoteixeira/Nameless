namespace Nameless;

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
/// Singleton + Null Object Pattern implementation for <see cref="IProgress{T}" />.
/// <list type="bullet">
///     <item>
///         <term>Singleton Pattern</term>
///         <description>
///             <a href="https://en.wikipedia.org/wiki/Singleton_pattern">See here</a>
///         </description>
///     </item>
///     <item>
///         <term>Null-Object Pattern</term>
///         <description>
///             <a href="https://en.wikipedia.org/wiki/Null_object_pattern">See here</a>
///         </description>
///     </item>
/// </list>
/// </summary>
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