using Microsoft.Extensions.Hosting;
using Nameless.Bootstrap;

namespace Nameless.WPF.Hosting;

// Simple wrapper to not pollute generic IHost with extensions.
public sealed class WinHost : IHost {
    private readonly IHost _current;

    public IServiceProvider Services => _current.Services;

    public WinHost(IHost current) {
        _current = current;
    }

    public void Dispose() {
        _current.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default) {
        await _current.WarmupAsync(context: [], cancellationToken);
        await _current.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default) {
        return _current.StopAsync(cancellationToken);
    }
}