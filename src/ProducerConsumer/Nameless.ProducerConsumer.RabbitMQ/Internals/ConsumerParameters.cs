namespace Nameless.ProducerConsumer.RabbitMQ {
    internal class ConsumerParameters {
        #region Internal Properties

        internal bool AckOnSuccess { get; }
        internal bool AckMultiple { get; }
        internal bool NAckOnFailure { get; }
        internal bool NAckMultiple { get; }
        internal bool AutoAck { get; }
        internal bool RequeueOnFailure { get; }

        #endregion

        #region Internal Constructors

        internal ConsumerParameters(IEnumerable<Parameter> parameters) {
            var dictionary = ParameterHelper.ToDictionary(parameters);

            AckOnSuccess = dictionary.Get<bool>(nameof(AckOnSuccess));
            AckMultiple = dictionary.Get<bool>(nameof(AckMultiple));
            NAckOnFailure = dictionary.Get<bool>(nameof(NAckOnFailure));
            NAckMultiple = dictionary.Get<bool>(nameof(NAckMultiple));
            AutoAck = dictionary.Get<bool>(nameof(AutoAck));
            RequeueOnFailure = dictionary.Get<bool>(nameof(RequeueOnFailure));
        }

        #endregion
    }
}
