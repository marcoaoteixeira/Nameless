using StackExchange.Redis;

namespace Nameless.Caching.Redis;

public sealed class ConnectionMultiplexerManager : IConnectionMultiplexerManager, IDisposable, IAsyncDisposable {
    private readonly IConfigurationOptionsFactory _configurationOptionsFactory;

    private ConfigurationOptions? _configurationOptions;
    private ConnectionMultiplexer? _multiplexer;
    private bool _disposed;

    public ConnectionMultiplexerManager(IConfigurationOptionsFactory configurationOptionsFactory) {
        _configurationOptionsFactory = Prevent.Argument.Null(configurationOptionsFactory);
    }

    ~ConnectionMultiplexerManager() {
        Dispose(disposing: false);
    }

    public async Task<IConnectionMultiplexer> GetConnectionMultiplexerAsync() {
        BlockAccessAfterDispose();

        return _multiplexer ??= await ConnectionMultiplexer.ConnectAsync(configuration: GetConfigurationOptions())
                                                           .ConfigureAwait(continueOnCapturedContext: false);
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _multiplexer?.Dispose();
        }

        _configurationOptions = null;
        _multiplexer = null;

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (_multiplexer is not null) {
            await _multiplexer.DisposeAsync()
                              .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(ConnectionMultiplexerManager));
        }
    }

    private ConfigurationOptions GetConfigurationOptions()
        => _configurationOptions ??= _configurationOptionsFactory.CreateConfigurationOptions();
}