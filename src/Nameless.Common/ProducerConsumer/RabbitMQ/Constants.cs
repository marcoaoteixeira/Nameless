namespace Nameless.ProducerConsumer.RabbitMQ;

internal static class Constants {
    internal static TimeSpan MessageConfirmationTimeout { get; } = TimeSpan.FromMilliseconds(15_000);

    internal static class Queues {
        internal const string Default = "q.default";
    }
}
