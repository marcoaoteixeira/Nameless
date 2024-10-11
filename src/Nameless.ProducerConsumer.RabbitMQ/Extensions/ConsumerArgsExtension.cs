namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ConsumerArgsExtension {
    /// <summary>
    /// Retrieves the queue name.
    /// </summary>
    /// <param name="self">The current <see cref="ConsumerArgs"/>.</param>
    /// <returns>A string representing the queue name.</returns>
    public static string GetQueueName(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.QUEUE_NAME] ?? string.Empty;

        return (string)arg;
    }

    public static ConsumerArgs SetQueueName(this ConsumerArgs self, string value) {
        self[Internals.Constants.ConsumerArgsTokens.QUEUE_NAME] = value;

        return self;
    }

    public static bool GetAckOnSuccess(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.ACK_ON_SUCCESS] ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetAckOnSuccess(this ConsumerArgs self, bool value) {
        self[Internals.Constants.ConsumerArgsTokens.ACK_ON_SUCCESS] = value;

        return self;
    }

    public static bool GetAckMultiple(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.ACK_MULTIPLE] ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetAckMultiple(this ConsumerArgs self, bool value) {
        self[Internals.Constants.ConsumerArgsTokens.ACK_MULTIPLE] = value;

        return self;
    }

    public static bool GetNAckOnFailure(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.NACK_ON_FAILURE] ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetNAckOnFailure(this ConsumerArgs self, bool value) {
        self[Internals.Constants.ConsumerArgsTokens.NACK_ON_FAILURE] = value;

        return self;
    }

    public static bool GetNAckMultiple(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.NACK_MULTIPLE] ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetNAckMultiple(this ConsumerArgs self, bool value) {
        self[Internals.Constants.ConsumerArgsTokens.NACK_MULTIPLE] = value;

        return self;
    }

    public static bool GetAutoAck(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.AUTO_ACK] ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetAutoAck(this ConsumerArgs self, bool value) {
        self[Internals.Constants.ConsumerArgsTokens.AUTO_ACK] = value;

        return self;
    }

    public static bool GetRequeueOnFailure(this ConsumerArgs self) {
        var arg = self[Internals.Constants.ConsumerArgsTokens.REQUEUE_ON_FAILURE] ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetRequeueOnFailure(this ConsumerArgs self, bool value) {
        self[Internals.Constants.ConsumerArgsTokens.REQUEUE_ON_FAILURE] = value;

        return self;
    }
}