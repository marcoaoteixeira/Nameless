using System.Diagnostics;

namespace Nameless.Diagnostics;

public sealed class NullActivitySource : IActivitySource {
    public static IActivitySource Instance { get; } = new NullActivitySource();

    static NullActivitySource() {

    }

    private NullActivitySource() {

    }

    public string Name => string.Empty;

    public string? Version => null;

    public IActivity StartActivity(string name, ActivityKind kind, ActivityContext? parentContext) {
        return NullActivity.Instance;
    }

    public void Dispose() {

    }
}