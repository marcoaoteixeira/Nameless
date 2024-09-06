namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ConsumerArgsExtension {
    #region Public Static Methods

    /// <summary>
    /// Retrieves the queue name.
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string GetQueueName(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.QUEUE_NAME)
               ?? string.Empty;

        return (string)arg;
    }

    public static ConsumerArgs SetQueueName(this ConsumerArgs self, string value) {
        self.Set(Root.ConsumerArgsTokens.QUEUE_NAME, value);
        return self;
    }

    public static bool GetAckOnSuccess(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.ACK_ON_SUCCESS) ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetAckOnSuccess(this ConsumerArgs self, bool value) {
        self.Set(Root.ConsumerArgsTokens.ACK_ON_SUCCESS, value);
        return self;
    }

    public static bool GetAckMultiple(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.ACK_MULTIPLE) ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetAckMultiple(this ConsumerArgs self, bool value) {
        self.Set(Root.ConsumerArgsTokens.ACK_MULTIPLE, value);
        return self;
    }

    public static bool GetNAckOnFailure(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.NACK_ON_FAILURE) ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetNAckOnFailure(this ConsumerArgs self, bool value) {
        self.Set(Root.ConsumerArgsTokens.NACK_ON_FAILURE, value);
        return self;
    }

    public static bool GetNAckMultiple(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.NACK_MULTIPLE) ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetNAckMultiple(this ConsumerArgs self, bool value) {
        self.Set(Root.ConsumerArgsTokens.NACK_MULTIPLE, value);
        return self;
    }

    public static bool GetAutoAck(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.AUTO_ACK) ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetAutoAck(this ConsumerArgs self, bool value) {
        self.Set(Root.ConsumerArgsTokens.AUTO_ACK, value);
        return self;
    }

    public static bool GetRequeueOnFailure(this ConsumerArgs self) {
        var arg = self.Get(Root.ConsumerArgsTokens.REQUEUE_ON_FAILURE) ?? false;

        return (bool)arg;
    }

    public static ConsumerArgs SetRequeueOnFailure(this ConsumerArgs self, bool value) {
        self.Set(Root.ConsumerArgsTokens.REQUEUE_ON_FAILURE, value);
        return self;
    }

    #endregion
}