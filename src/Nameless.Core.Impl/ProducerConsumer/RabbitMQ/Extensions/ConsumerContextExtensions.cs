namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ConsumerContextExtensions {
    private const string QUEUE_NAME = "QueueName";

    private const string ACK_ON_SUCCESS = "AckOnSuccess";
    private const string ACK_MULTIPLE = "AckMultiple";

    private const string NACK_ON_FAILURE = "NAckOnFailure";
    private const string NACK_MULTIPLE = "NAckMultiple";

    private const string AUTO_ACK = "AutoAck";
    private const string REQUEUE_ON_FAILURE = "RequeueOnFailure";

    extension(ConsumerContext self) {
        public string QueueName {
            get => self[QUEUE_NAME] as string ?? "q.default";
            set => self[QUEUE_NAME] = value;
        }

        public bool AckOnSuccess {
            get => self[ACK_ON_SUCCESS] is true;
            set => self[ACK_ON_SUCCESS] = value;
        }

        public bool AckMultiple {
            get => self[ACK_MULTIPLE] is true;
            set => self[ACK_MULTIPLE] = value;
        }

        public bool NAckOnFailure {
            get => self[NACK_ON_FAILURE] is true;
            set => self[NACK_ON_FAILURE] = value;
        }

        public bool NAckMultiple {
            get => self[NACK_MULTIPLE] is true;
            set => self[NACK_MULTIPLE] = value;
        }

        public bool AutoAck {
            get => self[AUTO_ACK] is true;
            set => self[AUTO_ACK] = value;
        }

        public bool RequeueOnFailure {
            get => self[REQUEUE_ON_FAILURE] is true;
            set => self[REQUEUE_ON_FAILURE] = value;
        }
    }
}
