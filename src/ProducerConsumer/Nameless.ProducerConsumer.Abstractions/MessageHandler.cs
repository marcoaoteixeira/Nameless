namespace Nameless.ProducerConsumer {
    public delegate Task MessageHandler<in T>(T message);
}
