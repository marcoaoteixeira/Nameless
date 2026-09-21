using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     RabbitMQ implementation of <see cref="IProducer"/> that manages a per-topic channel
///     cache and publishes serialized messages to the broker.
/// </summary>
public sealed class Producer : IProducer, IDisposable, IAsyncDisposable {
    private readonly IChannelFactory _channelFactory;
    private readonly IMessageSerializer _serializer;
    private readonly Dictionary<string, QueueOptions> _queues;
    private readonly ILogger<Producer> _logger;

    private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1);

    private Dictionary<string, ChannelCacheEntry> _cache = [];
    private int _disposed;

    /// <summary>
    ///     Initializes a new <see cref="Producer"/>.
    /// </summary>
    /// <param name="channelFactory">Factory used to create RabbitMQ channels per topic.</param>
    /// <param name="options">RabbitMQ options.</param>
    /// <param name="serializer">Serializer for encoding messages before publishing.</param>
    /// <param name="logger">Logger for this producer instance.</param>
    public Producer(IChannelFactory channelFactory, IMessageSerializer serializer, IOptions<RabbitMQOptions> options, ILogger<Producer> logger) {
        _channelFactory = channelFactory;
        _serializer = serializer;
        _queues = options.Value.Queues.ToDictionary(
            keySelector: queue => queue.Name,
            elementSelector: queue => queue
        );
        _logger = logger;
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~Producer() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public async Task ProduceAsync<T>(string topic, T value, ProducerContext context, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        var entry = await GetChannelCacheEntryAsync(topic, cancellationToken).SkipContextSync();

        await InnerProduceAsync(entry, value, context, cancellationToken).SkipContextSync();
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private async Task<ChannelCacheEntry> GetChannelCacheEntryAsync(string topic, CancellationToken cancellationToken) {
        await _semaphore.WaitAsync(cancellationToken);

        try {
            if (_cache.TryGetValue(topic, out var entry)) {
                return entry;
            }

            if (!_queues.TryGetValue(topic, out var queue)) {
                throw new MissingQueueConfigurationException(topic);
            }

            var channel = await _channelFactory.CreateAsync(topic, cancellationToken)
                                               .SkipContextSync();

            return _cache[topic] = new ChannelCacheEntry {
                Topic = topic,
                Channel = channel,
                Options = queue
            };
        }
        catch (Exception ex) { CommonLog.Error(_logger, ex.Message, ex, tag: GetType().Tag); throw; }
        finally { _semaphore.Release(); }
    }

    private async Task InnerProduceAsync<T>(ChannelCacheEntry entry, T value, ProducerContext context, CancellationToken cancellationToken) {
        try { await entry.Lock.WaitAsync(cancellationToken); }
        catch(Exception ex) { Log.UnableAcquireProducerSemaphore(_logger, ex, GetType().Tag); return; }

        try {
            var properties = context.CreateBasicProperties();
            var buffer = _serializer.Serialize(value, context);

            await entry.Channel.BasicPublishAsync(
                entry.Options.ExchangeName,
                entry.Topic,
                context.Mandatory,
                properties,
                buffer,
                cancellationToken
            ).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception ex) { CommonLog.Error(_logger, ex.Message, ex, GetType().Tag); throw; }
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

    private Dictionary<string, ChannelCacheEntry> SwapCache() {
        return Interlocked.Exchange(ref _cache, new Dictionary<string, ChannelCacheEntry>());
    }

    private void BlockAccessAfterDispose() {
        if (Volatile.Read(ref _disposed) == 1) {
            throw new ObjectDisposedException(nameof(Producer));
        }
    }

    internal class ChannelCacheEntry {
        internal required string Topic { get; init; }
        internal required QueueOptions Options { get; init; }
        internal required IChannel Channel { get; init; }
        internal SemaphoreSlim Lock { get; } = new(initialCount: 1, maxCount: 1);
    }
}