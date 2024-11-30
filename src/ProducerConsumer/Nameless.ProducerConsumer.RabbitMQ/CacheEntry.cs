using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed record CacheEntry : IDisposable, IAsyncDisposable {
    private readonly string _tag;

    private IDisposable? _registration;
    private IChannel? _channel;

    private bool _disposed;

    public IDisposable Registration
        => Prevent.Argument.Null(_registration);
    public IChannel Channel
        => Prevent.Argument.Null(_channel);

    public CacheEntry(IDisposable registration, IChannel channel) {
        _registration = Prevent.Argument.Null(registration);
        _channel = Prevent.Argument.Null(channel);

        _tag = _registration.ToString() ?? string.Empty;
    }

    ~CacheEntry() {
        Dispose(disposing: false);
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
            _channel?.Dispose();
            _registration?.Dispose();
        }

        _channel = null;
        _registration = null;

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (_channel is not null) {
            await _channel.CloseAsync(global::RabbitMQ.Client.Constants.ReplySuccess, $"Disposing RabbitMQ channel for registration '{_tag}'.")
                          .ConfigureAwait(continueOnCapturedContext: false);
            await _channel.DisposeAsync()
                          .ConfigureAwait(continueOnCapturedContext: false);
        }

        _registration?.Dispose();
    }
}
