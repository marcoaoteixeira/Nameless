namespace Nameless.ProducerConsumer.RabbitMQ {
    public static class ConsumerArgsExtension {
        #region Public Static Methods

        public static bool GetAckOnSuccess(this ConsumerArgs self) {
            var arg = self.Get(Root.ConsumerArgsTokens.ACK_ON_SUCCESS) ?? false;

            return (bool)arg;
        }

        public static void SetAckOnSuccess(this ConsumerArgs self, bool value)
            => self.Set(Root.ConsumerArgsTokens.ACK_ON_SUCCESS, value);

        public static bool GetAckMultiple(this ConsumerArgs self) {
            var arg = self.Get(Root.ConsumerArgsTokens.ACK_MULTIPLE) ?? false;

            return (bool)arg;
        }

        public static void SetAckMultiple(this ConsumerArgs self, bool value)
            => self.Set(Root.ConsumerArgsTokens.ACK_MULTIPLE, value);

        public static bool GetNAckOnFailure(this ConsumerArgs self) {
            var arg = self.Get(Root.ConsumerArgsTokens.NACK_ON_FAILURE) ?? false;

            return (bool)arg;
        }

        public static void SetNAckOnFailure(this ConsumerArgs self, bool value)
            => self.Set(Root.ConsumerArgsTokens.NACK_ON_FAILURE, value);

        public static bool GetNAckMultiple(this ConsumerArgs self) {
            var arg = self.Get(Root.ConsumerArgsTokens.NACK_MULTIPLE) ?? false;

            return (bool)arg;
        }

        public static void SetNAckMultiple(this ConsumerArgs self, bool value)
            => self.Set(Root.ConsumerArgsTokens.NACK_MULTIPLE, value);

        public static bool GetAutoAck(this ConsumerArgs self) {
            var arg = self.Get(Root.ConsumerArgsTokens.AUTO_ACK) ?? false;

            return (bool)arg;
        }

        public static void SetAutoAck(this ConsumerArgs self, bool value)
            => self.Set(Root.ConsumerArgsTokens.AUTO_ACK, value);

        public static bool GetRequeueOnFailure(this ConsumerArgs self) {
            var arg = self.Get(Root.ConsumerArgsTokens.REQUEUE_ON_FAILURE) ?? false;

            return (bool)arg;
        }

        public static void SetRequeueOnFailure(this ConsumerArgs self, bool value)
            => self.Set(Root.ConsumerArgsTokens.REQUEUE_ON_FAILURE, value);

        #endregion
    }
}
