using Microsoft.Extensions.Hosting;
using Nameless.Bootstrap;

namespace Nameless.Windows.Hosting.Wrappers;

// Simple wrapper to not pollute generic IHost with extensions.
public sealed class WinHost : IHost {
    private readonly IHost _current;

    public IServiceProvider Services => _current.Services;
    public Func<IServiceProvider, CancellationToken, Task>? OnStart { get; set; }
    public Func<IServiceProvider, CancellationToken, Task>? OnStop { get; set; }

    public WinHost(IHost current) {
        _current = current;
    }

    public void Dispose() {
        _current.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default) {
        await _current.WarmupAsync(cancellationToken);
        await _current.StartAsync(cancellationToken);

        if (OnStart is not null) {
            await OnStart(_current.Services, cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default) {
        if (OnStop is not null) {
            await OnStop(_current.Services, cancellationToken);
        }

        await _current.StopAsync(cancellationToken);
    }
}