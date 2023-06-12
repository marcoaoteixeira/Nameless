namespace Nameless.ProducerConsumer.RabbitMQ {

    public static class ArgumentsExtension {

        #region Public Static Methods

        public static bool AckOnSuccess(this Arguments self) => self.Get(ArgumentKeys.ACK_ON_SUCCESS, false);
        public static bool RequeueOnFailure(this Arguments self) => self.Get(ArgumentKeys.REQUEUE_ON_FAILURE, false);
        public static string QueueName(this Arguments self) => self.Get(ArgumentKeys.QUEUE_NAME, Constants.DEFAULT_QUEUE_NAME);
        public static string ExchangeName(this Arguments self) => self.Get(ArgumentKeys.EXCHANGE_NAME, Constants.DEFAULT_EXCHANGE_NAME);
        public static string? RoutingKey(this Arguments self) => self.Get(ArgumentKeys.ROUTING_KEY, (string?)default);
        public static bool AutoAck(this Arguments self) => self.Get(ArgumentKeys.AUTO_ACK, false);
        public static bool AckMultiple(this Arguments self) => self.Get(ArgumentKeys.ACK_MULTIPLE, false);
        public static bool RejectOnFailure(this Arguments self) => self.Get(ArgumentKeys.REJECT_ON_FAILURE, true);

        #endregion
    }
}
