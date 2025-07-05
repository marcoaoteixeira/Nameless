namespace Nameless.Null;

/// <summary>
///     Null implementation of <see cref="IServiceProvider"/> that does not
///     provide any services.
/// </summary>
public sealed class NullServiceProvider : IServiceProvider {
    /// <summary>
    ///     Gets the unique instance of <see cref="NullServiceProvider"/>.
    /// </summary>
    public static IServiceProvider Instance { get; } = new NullServiceProvider();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullServiceProvider() { }

    private NullServiceProvider() { }

    /// <inheritdoc />
    public object? GetService(Type serviceType) {
        return null;
    }
}
