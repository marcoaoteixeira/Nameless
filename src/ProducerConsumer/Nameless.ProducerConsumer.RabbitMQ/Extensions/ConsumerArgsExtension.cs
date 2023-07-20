namespace Nameless.ProducerConsumer.RabbitMQ {
    public static class ConsumerArgsExtension {
        #region Private Constants

        private const string ACK_ON_SUCCESS = "AckOnSuccess";
        private const string ACK_MULTIPLE = "AckMultiple";

        private const string NACK_ON_FAILURE = "NAckOnFailure";
        private const string NACK_MULTIPLE = "NAckMultiple";

        private const string AUTO_ACK = "AutoAck";
        private const string REQUEUE_ON_FAILURE = "RequeueOnFailure";

        #endregion

        #region Public Static Methods

        public static bool GetAckOnSuccess(this ConsumerArgs self) {
            var arg = self.Get(ACK_ON_SUCCESS) ?? false;

            return (bool)arg;
        }

        public static void SetAckOnSuccess(this ConsumerArgs self, bool value) {
            self.Set(ACK_ON_SUCCESS, value);
        }

        public static bool GetAckMultiple(this ConsumerArgs self) {
            var arg = self.Get(ACK_MULTIPLE) ?? false;

            return (bool)arg;
        }

        public static void SetAckMultiple(this ConsumerArgs self, bool value) {
            self.Set(ACK_MULTIPLE, value);
        }

        public static bool GetNAckOnFailure(this ConsumerArgs self) {
            var arg = self.Get(NACK_ON_FAILURE) ?? false;

            return (bool)arg;
        }

        public static void SetNAckOnFailure(this ConsumerArgs self, bool value) {
            self.Set(NACK_ON_FAILURE, value);
        }

        public static bool GetNAckMultiple(this ConsumerArgs self) {
            var arg = self.Get(NACK_MULTIPLE) ?? false;

            return (bool)arg;
        }

        public static void SetNAckMultiple(this ConsumerArgs self, bool value) {
            self.Set(NACK_MULTIPLE, value);
        }

        public static bool GetAutoAck(this ConsumerArgs self) {
            var arg = self.Get(AUTO_ACK) ?? false;

            return (bool)arg;
        }

        public static void SetAutoAck(this ConsumerArgs self, bool value) {
            self.Set(AUTO_ACK, value);
        }

        public static bool GetRequeueOnFailure(this ConsumerArgs self) {
            var arg = self.Get(REQUEUE_ON_FAILURE) ?? false;

            return (bool)arg;
        }

        public static void SetRequeueOnFailure(this ConsumerArgs self, bool value) {
            self.Set(REQUEUE_ON_FAILURE, value);
        }

        #endregion
    }
}
