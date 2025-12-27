using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

internal static class BasicPropertiesExtensions {
    extension(BasicProperties self) {
        internal BasicProperties FillWith(Parameters parameters) {
            self.AppId = parameters.GetAppId();
            self.ClusterId = parameters.GetClusterId();
            self.ContentEncoding = parameters.GetContentEncoding();
            self.ContentType = parameters.GetContentType();
            self.CorrelationId = parameters.GetCorrelationId();
            self.DeliveryMode = parameters.GetDeliveryMode();
            self.Expiration = parameters.GetExpiration();
            self.Headers = parameters.GetHeaders();
            self.MessageId = parameters.GetMessageId();
            self.Persistent = parameters.GetPersistent();
            self.Priority = parameters.GetPriority();
            self.ReplyTo = parameters.GetReplyTo();

            if (parameters.GetReplyToAddress() is not null) {
                self.ReplyToAddress = parameters.GetReplyToAddress();
            }

            self.Timestamp = parameters.GetTimestamp();
            self.Type = parameters.GetTypeProp();
            self.UserId = parameters.GetUserId();

            return self;
        }
    }
}