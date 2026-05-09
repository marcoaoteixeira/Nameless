namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Extension properties for <see cref="ConsumerContext"/> that expose
///     RabbitMQ consumer-specific settings stored in the context dictionary.
/// </summary>
public static class ConsumerContextExtensions {
    private const string QUEUE_NAME = "QueueName";

    private const string ACK_ON_SUCCESS = "AckOnSuccess";
    private const string ACK_MULTIPLE = "AckMultiple";

    private const string NACK_ON_FAILURE = "NAckOnFailure";
    private const string NACK_MULTIPLE = "NAckMultiple";

    private const string AUTO_ACK = "AutoAck";
    private const string REQUEUE_ON_FAILURE = "RequeueOnFailure";

    /// <param name="self">The current <see cref="ConsumerContext"/> instance.</param>
    extension(ConsumerContext self) {
        /// <summary>
        ///     Gets or sets the queue name associated with this consumer context.
        /// </summary>
        public string QueueName {
            get => self[QUEUE_NAME] as string ?? "q.default";
            set => self[QUEUE_NAME] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the message should be acknowledged on successful processing.
        /// </summary>
        public bool AckOnSuccess {
            get => self[ACK_ON_SUCCESS] is true;
            set => self[ACK_ON_SUCCESS] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the acknowledgement should apply to all
        ///     outstanding messages up to and including this one.
        /// </summary>
        public bool AckMultiple {
            get => self[ACK_MULTIPLE] is true;
            set => self[ACK_MULTIPLE] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the message should be negatively acknowledged on failure.
        /// </summary>
        public bool NAckOnFailure {
            get => self[NACK_ON_FAILURE] is true;
            set => self[NACK_ON_FAILURE] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the negative acknowledgement should apply to all
        ///     outstanding messages up to and including this one.
        /// </summary>
        public bool NAckMultiple {
            get => self[NACK_MULTIPLE] is true;
            set => self[NACK_MULTIPLE] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether messages are automatically acknowledged upon delivery.
        /// </summary>
        public bool AutoAck {
            get => self[AUTO_ACK] is true;
            set => self[AUTO_ACK] = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether failed messages should be requeued.
        /// </summary>
        public bool RequeueOnFailure {
            get => self[REQUEUE_ON_FAILURE] is true;
            set => self[REQUEUE_ON_FAILURE] = value;
        }
    }
}
