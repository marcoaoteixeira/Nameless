using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {

    public static class ArgumentKeys {

        #region Public Constants

        public const string EXCHANGE_NAME = nameof(EXCHANGE_NAME);
        public const string ROUTING_KEY = nameof(ROUTING_KEY);
        public const string ACK_ON_SUCCESS = nameof(ACK_ON_SUCCESS);
        public const string QUEUE_NAME = nameof(QUEUE_NAME);
        public const string REQUEUE_ON_FAILURE = nameof(REQUEUE_ON_FAILURE);
        public const string AUTO_ACK = nameof(AUTO_ACK);
        public const string ACK_MULTIPLE = nameof(ACK_MULTIPLE);
        public const string REJECT_ON_FAILURE = nameof(REJECT_ON_FAILURE);

        public const string BP_APPID = nameof(IBasicProperties.AppId);
        public const string BP_CLUSTERID = nameof(IBasicProperties.ClusterId);
        public const string BP_CONTENTENCODING = nameof(IBasicProperties.ContentEncoding);
        public const string BP_CONTENTTYPE = nameof(IBasicProperties.ContentType);
        public const string BP_CORRELATIONID = nameof(IBasicProperties.CorrelationId);
        public const string BP_DELIVERYMODE = nameof(IBasicProperties.DeliveryMode);
        public const string BP_EXPIRATION = nameof(IBasicProperties.Expiration);
        public const string BP_HEADERS = nameof(IBasicProperties.Headers);
        public const string BP_MESSAGEID = nameof(IBasicProperties.MessageId);
        public const string BP_PERSISTENT = nameof(IBasicProperties.Persistent);
        public const string BP_PRIORITY = nameof(IBasicProperties.Priority);
        public const string BP_REPLYTO = nameof(IBasicProperties.ReplyTo);
        public const string BP_REPLYTOADDRESS = nameof(IBasicProperties.ReplyToAddress);
        public const string BP_TIMESTAMP = nameof(IBasicProperties.Timestamp);
        public const string BP_TYPE = nameof(IBasicProperties.Type);
        public const string BP_USERID = nameof(IBasicProperties.UserId);

        #endregion
    }
}
