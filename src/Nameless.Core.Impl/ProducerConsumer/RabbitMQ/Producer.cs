using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class Producer : IProducer, IDisposable, IAsyncDisposable {
    private readonly IChannelFactory _channelFactory;
    private readonly IConfiguration _configuration;
    private readonly IMessageSerializer _serializer;
    private readonly ILogger<Producer> _logger;

    private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1);

    private Dictionary<string, CacheEntry> _cache = [];
    private int _disposed;

    public Producer(IChannelFactory channelFactory, IConfiguration configuration, IMessageSerializer serializer, ILogger<Producer> logger) {
        _channelFactory = channelFactory;
        _configuration = configuration;
        _serializer = serializer;
        _logger = logger;
    }

    ~Producer() {
        Dispose(disposing: false);
    }

    public async Task ProduceAsync(string topic, object message, ProducerContext context, CancellationToken cancellationToken) {
        var entry = await FetchCacheEntryAsync(topic, cancellationToken).SkipContextSync();

        await InnerProduceAsync(entry, message, context, cancellationToken).SkipContextSync();
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

    private async Task<CacheEntry> FetchCacheEntryAsync(string topic, CancellationToken cancellationToken) {
        await _semaphore.WaitAsync(cancellationToken);

        try {
            BlockAccessAfterDispose();

            if (_cache.TryGetValue(topic, out var entry)) {
                return entry;
            }

            var options = _configuration.GetQueueOptions(topic);
            var channel = await _channelFactory.CreateAsync(topic, cancellationToken).SkipContextSync();

            return _cache[topic] = new CacheEntry {
                Topic = topic,
                Channel = channel,
                Options = options
            };
        }
        catch (Exception ex) { _logger.Failure(ex); throw; }
        finally { _semaphore.Release(); }
    }

    private async Task InnerProduceAsync(CacheEntry entry, object message, ProducerContext context, CancellationToken cancellationToken) {
        try { await entry.Lock.WaitAsync(cancellationToken); }
        catch { _logger.ChannelSemaphoreDisposed(); return; }

        try {
            BlockAccessAfterDispose();

            var properties = context.CreateBasicProperties();
            var buffer = await _serializer.SerializeAsync(
                message,
                context,
                cancellationToken
            ).SkipContextSync();

            await entry.Channel.BasicPublishAsync(
                entry.Options.ExchangeName,
                entry.Topic,
                context.Mandatory,
                properties,
                buffer,
                cancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex) { _logger.Failure(ex); throw; }
        finally { entry.Lock.Release(); }
    }

    private void Dispose(bool disposing) {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) { return; }

        _semaphore.Wait();

        try {
            if (disposing) {
                var snapshot = SwapCache();

                foreach (var item in snapshot.Values) {
                    item.Lock.Wait();
                    try { item.Channel.Dispose(); }
                    finally {
                        item.Lock.Release();
                        item.Lock.Dispose();
                    }
                }

                snapshot.Clear();
            }
        }
        finally { _semaphore.Release(); }

        _semaphore.Dispose();
    }

    private async ValueTask DisposeAsyncCore() {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) { return; }

        await _semaphore.WaitAsync().SkipContextSync();

        try {
            var snapshot = SwapCache();

            foreach (var item in snapshot.Values) {
                await item.Lock.WaitAsync().SkipContextSync();
                try {
                    await item.Channel
                              .DisposeAsync()
                              .ConfigureAwait(continueOnCapturedContext: false);
                }
                finally {
                    item.Lock.Release();
                    item.Lock.Dispose();
                }
            }

            snapshot.Clear();
        }
        finally { _semaphore.Release(); }

        _semaphore.Dispose();
    }

    private Dictionary<string, CacheEntry> SwapCache() {
        return Interlocked.Exchange(ref _cache, new Dictionary<string, CacheEntry>());
    }

    private void BlockAccessAfterDispose() {
        if (Volatile.Read(ref _disposed) == 1) {
            throw new ObjectDisposedException(nameof(Producer));
        }
    }

    internal class CacheEntry {
        internal required string Topic { get; init; }
        internal required QueueOptions Options { get; init; }
        internal required IChannel Channel { get; init; }
        internal SemaphoreSlim Lock { get; } = new(initialCount: 1, maxCount: 1);
    }
}