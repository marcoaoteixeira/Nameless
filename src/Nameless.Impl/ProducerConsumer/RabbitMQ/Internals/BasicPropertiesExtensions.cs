using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

internal static class BasicPropertiesExtensions {
    extension(BasicProperties self) {
        internal BasicProperties FillWith(Context ctx) {
            self.AppId = ctx.AppId;
            self.ClusterId = ctx.ClusterId;
            self.ContentEncoding = ctx.ContentEncoding;
            self.ContentType = ctx.ContentType;
            self.CorrelationId = ctx.CorrelationId;
            self.DeliveryMode = ctx.DeliveryMode;
            self.Expiration = ctx.Expiration;
            self.Headers = ctx.Headers;
            self.MessageId = ctx.MessageId;
            self.Persistent = ctx.Persistent;
            self.Priority = ctx.Priority;
            self.ReplyTo = ctx.ReplyTo;

            if (ctx.ReplyToAddress is not null) {
                self.ReplyToAddress = ctx.ReplyToAddress;
            }

            self.Timestamp = ctx.Timestamp;
            self.Type = ctx.Type;
            self.UserId = ctx.UserId;

            return self;
        }
    }

    extension(IReadOnlyBasicProperties self) {
        internal ConsumerContext ToConsumerContext() {
            var result = new ConsumerContext {
                AppId = self.AppId,
                ClusterId = self.ClusterId,
                ContentEncoding = self.ContentEncoding,
                ContentType = self.ContentType,
                CorrelationId = self.CorrelationId,
                DeliveryMode = self.DeliveryMode,
                Expiration = self.Expiration,
                MessageId = self.MessageId,
                Persistent = self.Persistent,
                Priority = self.Priority,
                ReplyTo = self.ReplyTo,
                Timestamp = self.Timestamp,
                Type = self.Type,
                UserId = self.UserId
            };

            if (self.Headers is not null) {
                result.Headers = self.Headers;
            }

            if (self.ReplyToAddress is not null) {
                result.ReplyToAddress = self.ReplyToAddress;
            }

            return result;
        }
    }
}