namespace Nameless.Null;

/// <summary>
///     Singleton + Null Object Pattern implementation for <see cref="IDisposable" />.
///     <list type="bullet">
///         <item>
///             <term>Singleton Pattern</term>
///             <description>
///                 <a href="https://en.wikipedia.org/wiki/Singleton_pattern">See here</a>
///             </description>
///         </item>
///         <item>
///             <term>Null-Object Pattern</term>
///             <description>
///                 <a href="https://en.wikipedia.org/wiki/Null_object_pattern">See here</a>
///             </description>
///         </item>
///     </list>
/// </summary>
public sealed class NullDisposable : IDisposable {
    /// <summary>
    ///     Gets the unique instance of <see cref="NullDisposable" />.
    /// </summary>
    public static IDisposable Instance { get; } = new NullDisposable();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullDisposable() { }

    private NullDisposable() { }

    public void Dispose() { }
}