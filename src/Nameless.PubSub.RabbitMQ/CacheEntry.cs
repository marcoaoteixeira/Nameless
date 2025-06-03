using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

internal sealed record CacheEntry : IDisposable, IAsyncDisposable {
    private readonly string _consumerTag;
    private IChannel? _channel;

    private bool _disposed;

    private Subscription? _subscription;

    public CacheEntry(Subscription subscription, IChannel channel) {
        _subscription = Prevent.Argument.Null(subscription);
        _channel = Prevent.Argument.Null(channel);

        _consumerTag = _subscription.ConsumerTag;
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CacheEntry() {
        Dispose(false);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _subscription?.Dispose();
            _channel?.Dispose();
        }

        _subscription = null;
        _channel = null;

        _disposed = true;
    }

    private async Task DisposeAsyncCore() {
        if (_channel is not null) {
            try {
                await _channel.BasicCancelAsync(_consumerTag, true)
                              .ConfigureAwait(false);
                await _channel.CloseAsync(Constants.ReplySuccess, $"Closing RabbitMQ channel for subscription '{_consumerTag}'.")
                              .ConfigureAwait(false);
            }
            catch {
                /* swallow */
            }

            await _channel.DisposeAsync()
                          .ConfigureAwait(false);
        }
    }
}