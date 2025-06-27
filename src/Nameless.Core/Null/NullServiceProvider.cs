namespace Nameless.Null;

/// <summary>
///     Null implementation of <see cref="IServiceProvider"/> that does not provide any services.
/// </summary>
public sealed class NullServiceProvider : IServiceProvider {
    public static IServiceProvider Instance { get; } = new NullServiceProvider();

    static NullServiceProvider() { }

    private NullServiceProvider() { }

    /// <inheritdoc />
    public object? GetService(Type serviceType) {
        return null;
    }
}
