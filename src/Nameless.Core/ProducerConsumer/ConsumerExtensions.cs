namespace Nameless.ProducerConsumer;

public static class ConsumerExtensions {
    extension<TMessage>(IConsumer<TMessage> self) {
        public Task HandleAsync(TMessage message, CancellationToken cancellationToken) {
            return self.ConsumeAsync(message, context: [], cancellationToken);
        }
    }
}