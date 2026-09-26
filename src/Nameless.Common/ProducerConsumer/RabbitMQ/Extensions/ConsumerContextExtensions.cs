namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Extension properties for <see cref="ConsumerContext"/> that expose
///     RabbitMQ consumer-specific settings stored in the context dictionary.
/// </summary>
public static class ConsumerContextExtensions {
    private const string QUEUE_NAME = "QueueName";
    private const string DELIVERY_TAG = "DeliveryTag";

    /// <param name="self">The current <see cref="ConsumerContext"/> instance.</param>
    extension(ConsumerContext self) {
        /// <summary>
        ///     Gets or sets the queue name associated with this consumer context.
        /// </summary>
        public string QueueName {
            get => self[QUEUE_NAME] as string ?? Constants.Queues.Default;
            set => self[QUEUE_NAME] = value;
        }

        /// <summary>
        ///     Gets or sets the consumer tag.
        /// </summary>
        public ulong DeliveryTag {
            get => self[DELIVERY_TAG] is ulong value ? value : 0L;
            set => self[DELIVERY_TAG] = value;
        }
    }
}
