namespace Nameless.Diagnostics;

public sealed class NullActivitySourceProvider : IActivitySourceProvider {
    public static IActivitySourceProvider Instance { get; } = new NullActivitySourceProvider();

    static NullActivitySourceProvider() {

    }

    private NullActivitySourceProvider() {

    }

    public IActivitySource Create(string name, string? version = null) {
        return NullActivitySource.Instance;
    }
}