namespace Nameless.ProducerConsumer {
    public delegate Task MessageHandler<T>(T message);
}
