using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ParametersExtensions {
    #region Basic Properties Parameters

    extension(Parameters self) {
        public string? GetAppId() {
            return self[nameof(IBasicProperties.AppId)] as string;
        }

        public void SetAppId(string? value) {
            self[nameof(IBasicProperties.AppId)] = value;
        }

        public string? GetClusterId() {
            return self[nameof(IBasicProperties.ClusterId)] as string;
        }

        public void SetClusterId(string? value) {
            self[nameof(IBasicProperties.ClusterId)] = value;
        }

        public string? GetContentEncoding() {
            return self[nameof(IBasicProperties.ContentEncoding)] as string;
        }

        public void SetContentEncoding(string? value) {
            self[nameof(IBasicProperties.ContentEncoding)] = value;
        }

        public string? GetContentType() {
            return self[nameof(IBasicProperties.ContentType)] as string;
        }

        public void SetContentType(string? value) {
            self[nameof(IBasicProperties.ContentType)] = value;
        }

        public string? GetCorrelationId() {
            return self[nameof(IBasicProperties.CorrelationId)] as string;
        }

        public void SetCorrelationId(string? value) {
            self[nameof(IBasicProperties.CorrelationId)] = value;
        }

        public DeliveryModes GetDeliveryMode() {
            return self[nameof(IBasicProperties.DeliveryMode)] is DeliveryModes modes ? modes : default;
        }

        public void SetDeliveryMode(byte? value) {
            self[nameof(IBasicProperties.DeliveryMode)] = value;
        }

        public string? GetExpiration() {
            return self[nameof(IBasicProperties.Expiration)] as string;
        }

        public void SetExpiration(string? value) {
            self[nameof(IBasicProperties.Expiration)] = value;
        }

        public IDictionary<string, object?> GetHeaders() {
            return self[nameof(IBasicProperties.Headers)] as Dictionary<string, object?> ?? [];
        }

        public void SetHeaders(IDictionary<string, object>? value) {
            self[nameof(IBasicProperties.Headers)] = value;
        }

        public string? GetMessageId() {
            return self[nameof(IBasicProperties.MessageId)] as string;
        }

        public void SetMessageId(string? value) {
            self[nameof(IBasicProperties.MessageId)] = value;
        }

        public bool GetPersistent() {
            return self[nameof(IBasicProperties.Persistent)] is true;
        }

        public void SetPersistent(bool? value) {
            self[nameof(IBasicProperties.Persistent)] = value;
        }

        public byte GetPriority() {
            return self[nameof(IBasicProperties.Priority)] is byte value ? value : (byte)0;
        }

        public void SetPriority(byte? value) {
            self[nameof(IBasicProperties.Priority)] = value;
        }

        public string? GetReplyTo() {
            return self[nameof(IBasicProperties.ReplyTo)] as string;
        }

        public void SetReplyTo(string? value) {
            self[nameof(IBasicProperties.ReplyTo)] = value;
        }

        public PublicationAddress? GetReplyToAddress() {
            return self[nameof(IBasicProperties.ReplyToAddress)] as PublicationAddress;
        }

        public void SetReplyToAddress(PublicationAddress? value) {
            self[nameof(IBasicProperties.ReplyToAddress)] = value;
        }

        public AmqpTimestamp GetTimestamp() {
            return self[nameof(IBasicProperties.Timestamp)] is AmqpTimestamp value ? value : default;
        }

        public void SetTimestamp(AmqpTimestamp? value) {
            self[nameof(IBasicProperties.Timestamp)] = value;
        }

        public string? GetTypeProp() {
            return self[nameof(IBasicProperties.Type)] as string;
        }

        public void SetTypeProp(string? value) {
            self[nameof(IBasicProperties.Type)] = value;
        }

        public string? GetUserId() {
            return self[nameof(IBasicProperties.UserId)] as string;
        }

        public void SetUserId(string? value) {
            self[nameof(IBasicProperties.UserId)] = value;
        }
    }

    #endregion

    #region Producer Parameters

    extension(Parameters self) {
        public string GetExchangeName() {
            return self[ProducerParametersNames.EXCHANGE_NAME] as string ?? Internals.Defaults.EXCHANGE_NAME;
        }

        public void SetExchangeName(string? value) {
            self[ProducerParametersNames.EXCHANGE_NAME] = value;
        }

        public bool HasRoutingKeys() {
            return self.GetRoutingKeys().Length > 0;
        }

        public string[] GetRoutingKeys() {
            return self[ProducerParametersNames.ROUTING_KEYS] as string[] ?? [];
        }

        public void SetRoutingKeys(string[]? value) {
            self[ProducerParametersNames.ROUTING_KEYS] = value;
        }

        public bool GetMandatory() {
            return self[ProducerParametersNames.MANDATORY] is true;
        }

        public void SetMandatory(bool? value) {
            self[ProducerParametersNames.MANDATORY] = value;
        }

        public bool GetUsePrefetch() {
            return self[ProducerParametersNames.USE_PREFETCH] is true;
        }

        public void SetUsePrefetch(bool? value) {
            self[ProducerParametersNames.USE_PREFETCH] = value;
        }
    }

    #endregion

    #region Consumer Parameters

    extension(Parameters self) {
        public string GetQueueName() {
            return self[ConsumerParameterNames.QUEUE_NAME] as string ?? Internals.Defaults.QUEUE_NAME;
        }

        public void SetQueueName(string? value) {
            self[ConsumerParameterNames.QUEUE_NAME] = value;
        }

        public bool GetAckOnSuccess() {
            return self[ConsumerParameterNames.ACK_ON_SUCCESS] is true;
        }

        public void SetAckOnSuccess(bool? value) {
            self[ConsumerParameterNames.ACK_ON_SUCCESS] = value;
        }

        public bool GetAckMultiple() {
            return self[ConsumerParameterNames.ACK_MULTIPLE] is true;
        }

        public void SetAckMultiple(bool? value) {
            self[ConsumerParameterNames.ACK_MULTIPLE] = value;
        }

        public bool GetNAckOnFailure() {
            return self[ConsumerParameterNames.NACK_ON_FAILURE] is true;
        }

        public void SetNAckOnFailure(bool? value) {
            self[ConsumerParameterNames.NACK_ON_FAILURE] = value;
        }

        public bool GetNAckMultiple() {
            return self[ConsumerParameterNames.NACK_MULTIPLE] is true;
        }

        public void SetNAckMultiple(bool? value) {
            self[ConsumerParameterNames.NACK_MULTIPLE] = value;
        }

        public bool GetAutoAck() {
            return self[ConsumerParameterNames.AUTO_ACK] is true;
        }

        public void SetAutoAck(bool? value) {
            self[ConsumerParameterNames.AUTO_ACK] = value;
        }

        public bool GetRequeueOnFailure() {
            return self[ConsumerParameterNames.REQUEUE_ON_FAILURE] is true;
        }

        public void SetRequeueOnFailure(bool? value) {
            self[ConsumerParameterNames.REQUEUE_ON_FAILURE] = value;
        }
    }

    #endregion
}