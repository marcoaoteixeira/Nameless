namespace Nameless.ProducerConsumer {
    public delegate Task MessageEventHandler<T>(T message, CancellationToken cancellationToken = default);
}
