using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

internal static class BasicPropertiesExtensions {
    internal static BasicProperties FillWith(this BasicProperties self, Args args) {
        self.AppId = args.GetAppId();
        self.ClusterId = args.GetClusterId();
        self.ContentEncoding = args.GetContentEncoding();
        self.ContentType = args.GetContentType();
        self.CorrelationId = args.GetCorrelationId();
        self.DeliveryMode = args.GetDeliveryMode();
        self.Expiration = args.GetExpiration();
        self.Headers = args.GetHeaders();
        self.MessageId = args.GetMessageId();
        self.Persistent = args.GetPersistent();
        self.Priority = args.GetPriority();
        self.ReplyTo = args.GetReplyTo();
        if (args.GetReplyToAddress() is not null) {
            self.ReplyToAddress = args.GetReplyToAddress();
        }

        self.Timestamp = args.GetTimestamp();
        self.Type = args.GetTypeProp();
        self.UserId = args.GetUserId();

        return self;
    }
}