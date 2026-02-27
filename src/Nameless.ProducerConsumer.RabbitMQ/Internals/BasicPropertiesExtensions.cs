using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

internal static class BasicPropertiesExtensions {
    extension(BasicProperties self) {
        internal BasicProperties FillWith(Parameters parameters) {
            self.AppId = parameters.AppId;
            self.ClusterId = parameters.ClusterId;
            self.ContentEncoding = parameters.ContentEncoding;
            self.ContentType = parameters.ContentType;
            self.CorrelationId = parameters.CorrelationId;
            self.DeliveryMode = parameters.DeliveryMode;
            self.Expiration = parameters.Expiration;
            self.Headers = parameters.Headers;
            self.MessageId = parameters.MessageId;
            self.Persistent = parameters.Persistent;
            self.Priority = parameters.Priority;
            self.ReplyTo = parameters.ReplyTo;

            if (parameters.ReplyToAddress is not null) {
                self.ReplyToAddress = parameters.ReplyToAddress;
            }

            self.Timestamp = parameters.Timestamp;
            self.Type = parameters.Type;
            self.UserId = parameters.UserId;

            return self;
        }
    }
}