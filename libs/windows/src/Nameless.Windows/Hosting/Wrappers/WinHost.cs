using Microsoft.Extensions.Hosting;

namespace Nameless.Windows.Hosting.Wrappers;

/// <summary>
///     Wrapper for Windows Hosting
/// </summary>
public sealed class WinHost : IHost {
    private readonly IHost _current;

    /// <summary>
    ///     Gets the registered services.
    /// </summary>
    public IServiceProvider Services => _current.Services;

    /// <summary>
    ///     Executes when the application starts.
    /// </summary>
    public Func<IServiceProvider, CancellationToken, Task>? OnStart { get; set; }

    /// <summary>
    ///     Executes when the application stops.
    /// </summary>
    public Func<IServiceProvider, CancellationToken, Task>? OnStop { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="WinHost"/> class.
    /// </summary>
    /// <param name="current">
    ///     The <see cref="IHost"/> instance to wrap.
    /// </param>
    public WinHost(IHost current) {
        _current = current;
    }

    /// <inheritdoc />
    public void Dispose() {
        _current.Dispose();
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default) {
        await _current.StartAsync(cancellationToken);

        if (OnStart is not null) {
            await OnStart(_current.Services, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default) {
        if (OnStop is not null) {
            await OnStop(_current.Services, cancellationToken);
        }

        await _current.StopAsync(cancellationToken);
    }
}